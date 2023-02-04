using Il2CppAssets.Scripts.Simulation.Towers.Behaviors.Attack;

namespace Diablo{
	[RegisterTypeInIl2Cpp]
    public class DiabloBehaviour:MonoBehaviour{
        public DiabloBehaviour(IntPtr ptr):base(ptr){}
		private int terror=0;
		public int Terror{
			get{
				return terror;
			}
			set{
				if(ShowTerrorInConsole){
					Log(terror+" "+value);
				}
				try{
					if(Math.Sign(value)==1){
						baseWeapon.attack.range+=0.075f;
						baseWeapon.CalcRateFrames()
					}else{
						baseWeapon.attack.range-=0.075f;
					}
				}catch{}
				terror=value;
			}
		}
		public int FireballCost=10;
		public int PrimeEvilCost=100;
		public int TerrorCap=0;
		public int UpgradeSound=0;
		public int SelectSound=0;
		public bool BonusDamageActive=false;
		public GameObject Base;
		public GameObject Prime;
		public ParticleSystem Fire;
		public ParticleSystem RuneCircle;
		public ParticleSystem RuneOverhead;
		public ParticleSystem RuneBottom;
		public GameObject ActiveForm;
		public List<Material>Materials=new();
		public bool TerrorizeActive=false;
		public bool FastAttack=false;
		public Weapon baseWeapon=null;
		void Start(){
			Base=transform.GetChild(0).gameObject;
			Prime=transform.GetChild(1).gameObject;
			Fire=transform.GetChild(2).GetComponent<ParticleSystem>();
			RuneCircle=transform.GetChild(3).GetComponent<ParticleSystem>();
			RuneOverhead=transform.GetChild(4).GetComponent<ParticleSystem>();
			RuneBottom=transform.GetChild(5).GetComponent<ParticleSystem>();
			ActiveForm=Base;
			Base.transform.localPosition=new(0,0,0);
			Prime.active=false;
			Prime.transform.localPosition=new(0,0,0);
			foreach(SkinnedMeshRenderer renderer in GetComponentsInChildren<SkinnedMeshRenderer>()){
				Materials.Add(renderer.material);
			}
			Terror=TerrorAmount;
		}
		public void PlaySelectSound(){
			if(SelectSound>5){
				SelectSound=0;
			}
			SelectSound+=1;
			if(ActiveForm.name.Contains("Base")){
				PlaySound("Diablo-BaseSelect"+SelectSound);
			}else{
				PlaySound("Diablo-PrimeSelect"+SelectSound);
			}
		}
		public void PlayUpgradeSound(){
			if(UpgradeSound>5){
				UpgradeSound=0;
			}
			UpgradeSound+=1;
			SelectSound=1;
			if(ActiveForm.name.Contains("Base")){
				PlaySound("Diablo-BaseUpgrade"+UpgradeSound);
			}else{
				PlaySound("Diablo-PrimeUpgrade"+UpgradeSound);
			}
		}
		float Timer=0;
        void Update(){
            Timer+=(Time.fixedDeltaTime/2);
            if(Timer>10){
				Animator animator=ActiveForm.GetComponent<Animator>();
                if(animator.GetCurrentStateName(0)=="StandReadyBase"){
                    switch(new System.Random().Next(1,101)){
                        case<14:
                            animator.CrossFade("Fidget",0.5f,0,0);
                            break;
                    }
                }
                Timer=0;
            }
			if(Terror>39){
				if(!RuneCircle.isPlaying){
					RuneCircle.Play();
					BonusDamageActive=true;
				}
			}else{
				if(RuneCircle.isPlaying){
					RuneCircle.Stop();
					BonusDamageActive=false;
				}
			}
			if(Terror>69){
				if(!RuneOverhead.isPlaying){
					RuneOverhead.Play();
				}
			}else{
				if(RuneOverhead.isPlaying){
					RuneOverhead.Stop();
				}
			}
			if(Terror>99){
				if(!RuneBottom.isPlaying){
					RuneBottom.Play();
				}
			}else{
				if(RuneBottom.isPlaying){
					RuneBottom.Stop();
				}
			}
		}
		bool thing=false;
		[HideFromIl2Cpp]
        public System.Collections.IEnumerator SwapForm(){
			Fire.Play(false);
            while(!thing){
                float visibility=1.1f;
                while(visibility>0){
                    foreach(Material material in Materials){
                        visibility=material.GetFloat("_Visibility");
                        material.SetFloat("_Visibility",visibility-0.01f);
                    }
                    yield return new WaitForSeconds(9999);
                }
				ActiveForm.active=false;
				if(ActiveForm.name.Contains("Base")){
					ActiveForm=Prime;
				}else{
					ActiveForm=Base;
				}
				Terror=0;
				ActiveForm.active=true;
                while(visibility<1){
                    foreach(Material material in Materials){
                        visibility=material.GetFloat("_Visibility");
                        material.SetFloat("_Visibility",visibility+0.01f);
                    }
                    yield return new WaitForSeconds(9999);
                }
                thing=true;
            }
			//handled here so that it plays later to avoid cutting the roar early
			if(ActiveForm.name.Contains("Prime")){
				PlaySound("Diablo-PrimeBirth");
			}
			SelectSound=0;
            thing=false;
        }
	}
}