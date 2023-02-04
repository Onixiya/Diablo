[assembly: MelonGame("Ninja Kiwi","BloonsTD6")]
[assembly: MelonInfo(typeof(Diablo.ModMain),"Diablo","1.0.0","Silentstorm")]
namespace Diablo{
    public class ModMain:BloonsTD6Mod{
        public static Dictionary<string,DHero>TowerTypes=new();
        public static AbilityModel BlankAbilityModel;
		public static string BundleDir;
        private static MelonLogger.Instance mllog;
        public static ModSettingBool NoTerrorCost=new(false){
			description="Disables terror requirements"
		};
		public static ModSettingInt TerrorAmount=new(0){
			description="Sets terror to whatever amount you put here"
		};
		public static ModSettingDouble PrimeEvilLifespan=new(20){
			description="Lifespan of Prime Evil",
			requiresRestart=true
		};
		public static ModSettingBool ShowTerrorInConsole=new(false){
			description="Whenever terror is set to a new value, log it"
		};
        public static void Log(object thingtolog,string type="msg"){
            switch(type){
                case"msg":
                    mllog.Msg(thingtolog);
                    break;
                case"warn":
                    mllog.Warning(thingtolog);
                    break;
                 case"error":
                    mllog.Error(thingtolog);
                    break;
            }
        }
        public override void OnInitialize(){
            mllog=LoggerInstance;
            BundleDir=MelonEnvironment.UserDataDirectory+"/DiabloBundles/";
            BundleDir=BundleDir.Replace('\\','/');
            Directory.CreateDirectory(BundleDir);
            Assembly assembly=MelonAssembly.Assembly;
            foreach(Type type in assembly.GetTypes()){
				try{
                    DHero tower=(DHero)Activator.CreateInstance(type);
                    if(tower.Name!=""){
                        TowerTypes.Add(tower.Name,tower);
                        tower.LoadedBundle=UnityEngine.AssetBundle.LoadFromFileAsync(BundleDir+tower.Name.ToLower()).assetBundle;
                        }
                }catch{}
            }
            foreach(string bundle in assembly.GetManifestResourceNames()){
                try{
                    Stream stream=assembly.GetManifestResourceStream(bundle);
                    byte[]bytes=new byte[stream.Length];
                    stream.Read(bytes);
                    File.WriteAllBytes(BundleDir+bundle.Split('.')[1],bytes);
                }catch(Exception error){
                    Log("Failed to write "+bundle);
                    string message=error.Message;
                    message+="@\n"+error.StackTrace;
                    Log(message,"error");
                }
            }
		}
        public static T LoadAsset<T>(string Asset,AssetBundle Bundle)where T:uObject{
            try{
                return Bundle.LoadAssetAsync(Asset,Il2CppType.Of<T>()).asset.Cast<T>();
            }catch{
                foreach(KeyValuePair<string,DHero>tower in TowerTypes){
                    tower.Value.LoadedBundle=UnityEngine.AssetBundle.LoadFromFileAsync(BundleDir+tower.Key.ToLower()).assetBundle;
                }
                try{
                    return Bundle.LoadAssetAsync(Asset,Il2CppType.Of<T>()).asset.Cast<T>();
                }catch(Exception error){
                    Log("Failed to load "+Asset+" from "+Bundle.name);
                    try{
                        Log("Attempting to get available assets");
                        foreach(string asset in Bundle.GetAllAssetNames()){
                            Log(asset);
                        }
                    }catch{
                        Log("Bundle is null");
                    }
                    string message=error.Message;
                    message+="@\n"+error.StackTrace;
                    Log(message,"error");
                    return null;
                }
            }
        }
        public static void PlaySound(string name){
            try{
                Game.instance.audioFactory.PlaySoundFromUnity(null,name,"FX",1,1);
            }catch(Exception error){
                Log("Failed to play clip "+name);
                string message=error.Message;
                message+="@\n"+error.StackTrace;
                Log(message,"error");
            }
        }
        public static void PlayAnimation(Animator animator,string anim,float duration=0.2f){
            try{
                if(animator!=null){
                    animator.CrossFade(anim,duration,0,0);
                }
            }catch(Exception error){
                Log("Failed to play animation "+anim);
                string message=error.Message;
                message+="@\n"+error.StackTrace;
                Log(message,"error");
            }
        }
        public override void OnGameModelLoaded(GameModel model){
            BlankAbilityModel=model.GetTowerFromId("Quincy 4").GetAbility().Duplicate();
            BlankAbilityModel.description="AbilityDescription";
            BlankAbilityModel.displayName="AbilityDisplayName";
            BlankAbilityModel.name="AbilityName";
            BlankAbilityModel.RemoveBehavior<TurboModel>();
            BlankAbilityModel.RemoveBehavior<CreateEffectOnAbilityModel>();
            BlankAbilityModel.RemoveBehavior<CreateSoundOnAbilityModel>();
        }
        [HarmonyPatch(typeof(Factory.__c__DisplayClass21_0),"_CreateAsync_b__0")]
        public class FactoryCreateAsync_Patch{
            [HarmonyPrefix]
            public static bool Prefix(ref Factory.__c__DisplayClass21_0 __instance,ref UnityDisplayNode prototype){
                string towerName=__instance.objectId.guidRef.Split('-')[0];
                if(towerName!=null&&TowerTypes.ContainsKey(towerName)){
                    try{
                        DHero tower=TowerTypes[towerName];
                        GameObject gObj=uObject.Instantiate(LoadAsset<GameObject>(__instance.objectId.guidRef,tower.LoadedBundle).Cast<GameObject>(),
                            __instance.__4__this.DisplayRoot);
                        gObj.name=__instance.objectId.guidRef;
                        gObj.transform.position=new(0,0,30000);
                        gObj.AddComponent<UnityDisplayNode>();
                        if(tower.Behaviours.ContainsKey(gObj.name)){
                            gObj.AddComponent(tower.Behaviours[gObj.name]);
                        }
                        prototype=gObj.GetComponent<UnityDisplayNode>();
                        __instance.__4__this.active.Add(prototype);
                        __instance.onComplete.Invoke(prototype);
                    }catch(Exception error){
                        Log("_CreateAsync_b__0: Failed to set "+__instance.objectId.guidRef+" up");
						Exception baseError=error.GetBaseException();
                        string message=baseError.Message;
                        message+="@\n"+baseError.StackTrace;
                        Log(message,"error");
                    }
                    return false;
                }
                return true;                
            }
        }
        [HarmonyPatch(typeof(UnityEngine.U2D.SpriteAtlas),"GetSprite")]
        public class SpriteAtlasGetSprite_Patch{
            [HarmonyPostfix]
            public static void Postfix(string name,ref Sprite __result){
                string towerName="";
                try{
                    towerName=name.Split('-')[0];
                }catch{}
                if(TowerTypes.ContainsKey(towerName)){
                    try{
                        Texture2D texture=LoadAsset<Texture2D>(name,TowerTypes[towerName].LoadedBundle).Cast<Texture2D>();
                        __result=Sprite.Create(texture,new(0,0,texture.width,texture.height),new());
                    }catch(Exception error){
                        Log("GetSprite: Failed to set "+name+" up");
						Exception baseError=error.GetBaseException();
                        string message=baseError.Message;
                        message+="@\n"+baseError.StackTrace;
                        Log(message,"error");
                    }
                }
            }
        }
        public override void OnWeaponFire(Weapon weapon){
            string towerName;
            try{
                towerName=weapon.attack.tower.towerModel.baseId.Split('-')[1];
            }catch{
                return;
            }
            if(TowerTypes.ContainsKey(towerName)&&TowerTypes[towerName].Attack!=null){
                TowerTypes[towerName].Attack(weapon);
            }
        }
        public override void OnTowerCreated(Tower tower,Entity target,Model modelToUse){
            string towerName;
            try{
                towerName=tower.towerModel.baseId.Split('-')[1];
            }catch{
                return;
            }
            if(TowerTypes.ContainsKey(towerName)&&TowerTypes[towerName].Create!=null){
                TowerTypes[towerName].Create(tower);
            }
        }
		[HarmonyPatch(typeof(Ability),"Activate")]
        public class AbilityActivate_Patch{
            [HarmonyPrefix]
            public static bool Prefix(Ability __instance){
				string towerName="";
				try{
                	towerName=__instance.tower.towerModel.baseId.Split('-')[1];
				}catch{}
                if(TowerTypes.ContainsKey(towerName)){
                    try{
                        return TowerTypes[towerName].Ability(__instance.abilityModel.name,__instance.tower);
                    }catch(Exception error){
                        Log("Failed to run Ability for "+towerName);
                        string message=error.Message;
                        message+="@\n"+error.StackTrace;
                        Log(message,"error");
						return true;
                    }
                }
				return true;
            }
        }
        [HarmonyPatch(typeof(AudioFactory),"Start")]
        public class AudioFactoryStart_Patch{
            [HarmonyPostfix]
            public static void Postfix(ref AudioFactory __instance){
                foreach(string bundlePath in Directory.GetFiles(BundleDir)){
                    if(bundlePath.EndsWith("clips")){
                        try{
                            foreach(uObject asset in AssetBundle.LoadFromFileAsync(bundlePath).assetBundle.LoadAllAssetsAsync<AudioClip>().allAssets){
                                __instance.RegisterAudioClip(asset.name,asset.Cast<AudioClip>());
                            }
                        }catch(Exception error){
                            Log("Failed to add audio clips from "+bundlePath);
                            string message=error.Message;
                            message+="@\n"+error.StackTrace;
                            Log(message,"error");
                        }
                    }
                }
            }
        }
        public override void OnTowerUpgraded(Tower tower,string upgradeName,TowerModel newBaseTowerModel){
            string towerName;
            try{
                towerName=tower.towerModel.baseId.Split('-')[1];
            }catch{
                return;
            }
            if(TowerTypes.ContainsKey(towerName)&&TowerTypes[towerName].Upgrade!=null){
                TowerTypes[towerName].Upgrade(upgradeName,tower);
            }
        }
        public override void OnTowerSelected(Tower tower){
            string towerName;
            try{
                towerName=tower.towerModel.baseId.Split('-')[1];
            }catch{
                return;
            }
            if(TowerTypes.ContainsKey(towerName)&&TowerTypes[towerName].Select!=null){
                TowerTypes[towerName].Select(tower);
            }
        }
        public override void OnTowerSold(Tower tower,float amount){
            string towerName;
            try{
                towerName=tower.towerModel.baseId.Split('-')[1];
            }catch{
                return;
            }
            if(TowerTypes.ContainsKey(towerName)&&TowerTypes[towerName].Sell!=null){
                TowerTypes[towerName].Sell(tower);
            }
        }
    }
}