namespace Diablo{
    public class Diablo:DHero{
        public override string BaseTower=>"PatFusty";
        public override int Cost=>750;
        public override string DisplayName=>"Diablo";
        public override string Title=>"Lord of Terror";
        public override string Level1Description=>"Smashes nearby bloons with demonic strength";
        public override string Description=>"Youngest of the Prime Evil's and lords of Hell, Diablo rules by fear and terror";
        public override string NameStyle=>TowerType.Gwendolin;
        public override string BackgroundStyle=>TowerType.Gwendolin;
        public override string GlowStyle=>TowerType.Gwendolin;
        public override Dictionary<int,SpriteReference>SelectScreenPortraits=>new(){
            {0,PortraitReference}
        };
        public override int MaxLevel=>20;
        public override float XpRatio=>1;
        public override SpriteReference PortraitReference=>new(){guidRef="Ui[Diablo-Portrait]"};
        public override SpriteReference ButtonReference=>new(){guidRef="Ui[Diablo-Button]"};
        public override SpriteReference SquareReference=>new(){guidRef="Ui[Diablo-HeroIcon]"};
        public override SpriteReference IconReference=>new(){guidRef="Ui[Diablo-Icon]"};
	 	public override Dictionary<string,Il2CppSystem.Type>Behaviours=>new(){{"Diablo-Prefab",Il2CppType.Of<DiabloBehaviour>()}};
        public override void ModifyBaseTowerModel(TowerModel diablo){
			diablo.icon=IconReference;
			diablo.portrait=PortraitReference;
			diablo.display=new(){guidRef="Diablo-Prefab"};
			diablo.GetBehavior<DisplayModel>().display=diablo.display;
			AttackModel attack=diablo.GetAttackModel();
			attack.name="BaseAttack";
			WeaponModel weapon=diablo.GetAttackModel().weapons[0];
			weapon.rate=1.25f;
			ProjectileModel proj=diablo.GetAttackModel().weapons[0].projectile;
			proj.GetDamageModel().damage=2;
			proj.pierce=5;
			proj.radius=15;
			proj.collisionPasses=new(new[]{0,-1});
			CreateProjectileOnExhaustFractionModel projExhaust=proj.GetBehavior<CreateProjectileOnExhaustFractionModel>();
			CreateProjectileOnContactModel projContact=new CreateProjectileOnContactModel("CPOCM",projExhaust.projectile.Duplicate(),projExhaust.emission.Duplicate(),
				true,false,false);
			projContact.projectile.pierce=5;
			projContact.projectile.radius=15;
			projContact.projectile.GetDamageModel().damage=2;
			projExhaust=null;
			projContact.projectile.RemoveBehavior<CreateEffectOnExhaustFractionModel>();
			projContact.projectile.collisionPasses=new(new[]{0,-1});
			proj.AddBehavior(projContact);
			proj.RemoveBehavior<CreateProjectileOnExhaustFractionModel>();
			proj.name="DiabloBaseProj";
			proj.id=proj.name;
			projContact.projectile.name="DiabloBaseAOEProj";
			projContact.projectile.id=projContact.projectile.name;
			diablo.RemoveBehavior<CreateSoundOnBloonLeakModel>();
			diablo.RemoveBehavior<CreateSoundOnBloonEnterTrackModel>();
			diablo.RemoveBehavior<CreateSoundOnUpgradeModel>();
			diablo.RemoveBehavior<CreateSoundOnTowerPlaceModel>();
			diablo.RemoveBehavior<CreateSoundOnSelectedModel>();
        }
        public class L2:ModHeroLevel<Diablo>{
            public override string Description=>"Faster attack speed";
            public override int Level=>2;
            public override void ApplyUpgrade(TowerModel diablo){
				diablo.GetAttackModel().weapons[0].rate=1.1f;
            }
        }
        public class L3:ModHeroLevel<Diablo>{
            public override string Description=>"Increases damage dealt";
            public override int Level=>3;
            public override void ApplyUpgrade(TowerModel diablo){
				ProjectileModel meleeProj=diablo.GetAttackModel().weapons[0].projectile;
				meleeProj.GetDamageModel().damage=4;
				meleeProj.GetBehavior<CreateProjectileOnContactModel>().projectile.GetDamageModel().damage=4;
            }
        }
        public class L4:ModHeroLevel<Diablo>{
            public override string Description=>"Starts gathering Terror from attacked bloons, gaining small buffs with each point of Terror";
            public override int Level=>4;
            public override void ApplyUpgrade(TowerModel diablo){
				//read upgrade override
            }
        }
        public class L5:ModHeroLevel<Diablo>{
            public override string Description=>"Hits more bloons at once";
            public override int Level=>5;
            public override void ApplyUpgrade(TowerModel diablo){
				ProjectileModel meleeProj=diablo.GetAttackModel().weapons[0].projectile;
				meleeProj.radius=20;
				meleeProj.GetBehavior<CreateProjectileOnContactModel>().projectile.pierce=20;
            }
        }
        public class L6:ModHeroLevel<Diablo>{
            public override string Description=>"Attacks further";
            public override int Level=>6;
            public override void ApplyUpgrade(TowerModel diablo){
				diablo.range=28;
				diablo.GetAttackModel().range=diablo.range;
            }
        }
        public class L7:ModHeroLevel<Diablo>{
            public override string Description=>"Destroys Lead Bloons instantly with fiery claws and increases damage";
            public override int Level=>7;
            public override void ApplyUpgrade(TowerModel diablo){
				ProjectileModel meleeProj=diablo.GetAttackModel().weapons[0].projectile;
				meleeProj.GetDamageModel().damage*=1.5f;
				meleeProj.GetBehavior<CreateProjectileOnContactModel>().projectile.GetDamageModel().damage*=1.5f;
				//read bloon damage patch for lead thing
            }
        }
        public class L8:ModHeroLevel<Diablo>{
            public override string AbilityName=>"Fireball";
            public override string AbilityDescription=>"Uses 10 Terror to cast a big ball of fire at the nearest bloon";
            public override string Description=>AbilityName+": "+AbilityDescription;
            public override int Level=>8;
            public override void ApplyUpgrade(TowerModel diablo){
				AbilityModel fireBall=BlankAbilityModel.Duplicate();
				fireBall.icon=new(){guidRef="Ui[Diablo-FireballIcon]"};
				fireBall.name="Fireball";
				fireBall.addedViaUpgrade="Diablo-Diablo Level 8";
				fireBall.cooldown=10;
				ActivateAttackModel fireBallActivate=Game.instance.model.GetTowerFromId("BombShooter-040").GetAbility().
					GetBehavior<ActivateAttackModel>().Duplicate();
				AttackModel fireBallAttack=Game.instance.model.GetTowerFromId("TackShooter-500").GetAttackModels().
					First(a=>a.name.Contains("Meteor")).Duplicate();
				fireBallAttack.name="FireballAttack";
				fireBallAttack.AddBehavior(diablo.GetAttackModel().GetBehavior<RotateToTargetModel>().Duplicate());
				WeaponModel fireBallWeapon=fireBallAttack.weapons[0];
				fireBallWeapon.rate=3;
				ProjectileModel fireBallProj=fireBallActivate.attacks[0].weapons[0].projectile;
				fireBallProj.name="DiabloFireballProj";
				fireBallProj.GetDamageModel().damage=50;
				fireBallProj.GetBehavior<CreateProjectileOnContactModel>().projectile.GetDamageModel().damage=25;
				fireBallActivate.attacks[0]=fireBallAttack;
				fireBall.AddBehavior(fireBallActivate);
				diablo.AddBehavior(fireBall);
            }
        }
        public class L9:ModHeroLevel<Diablo>{
            public override string Description=>"Gains bonus damage at 40 Terror";
            public override int Level=>9;
            public override void ApplyUpgrade(TowerModel diablo){
				//read the attack override bit
            }
        }
        public class L10:ModHeroLevel<Diablo>{
			public override string AbilityName=>"Prime Evil";
			public override string AbilityDescription=>"Uses 100 Terror to transform into the Prime Evil, massively buffing damage and gaining new attacks";
            public override string Description=>AbilityName+": "+AbilityDescription;
            public override int Level=>10;
            public override void ApplyUpgrade(TowerModel diablo){
				AbilityModel primeEvil=BlankAbilityModel.Duplicate();
				primeEvil.name="PrimeEvil";
				primeEvil.icon=new(){guidRef="Ui[Diablo-PrimeEvilIcon]"};
				primeEvil.cooldown=1;
				primeEvil.addedViaUpgrade="Diablo-Diablo Level 10";
				primeEvil.description=AbilityDescription;
				primeEvil.displayName=AbilityName;
				ActivateAttackModel activateAttack=Game.instance.model.GetTowerFromId("Alchemist-040").GetAbility().
					GetBehavior<ActivateAttackModel>().Duplicate();
				AttackModel attackFire=Game.instance.model.GetTowerFromId("WizardMonkey-030").GetAttackModels().
					First(a=>a.name.Contains("Breath")).Duplicate();
				attackFire.name="FireBreath";
				attackFire.range=60;
				attackFire.GetBehavior<AttackFilterModel>().filters.AddItem(new FilterOutTagModel("FOTM","Moabs",new(0)));
				AttackModel attackMelee=diablo.GetAttackModel().Duplicate();
				attackMelee.GetBehavior<AttackFilterModel>().filters.AddItem(new FilterWithTagModel("FWTM","Moabs",true));
				ProjectileModel meleeProj=attackMelee.weapons[0].projectile;
				meleeProj.GetDamageModel().damage=20;
				meleeProj.GetBehavior<CreateProjectileOnContactModel>().projectile.GetDamageModel().damage=20;
				activateAttack.attacks=new(new[]{attackFire,attackMelee});
				//not sure if its already true and really cannot be fucked checking
				activateAttack.turnOffExisting=true;
				activateAttack.lifespan=PrimeEvilLifespan;
				primeEvil.AddBehavior(activateAttack);
				diablo.AddBehavior(primeEvil);
            }
        }
        public class L11:ModHeroLevel<Diablo>{
            public override string Description=>"Increases the radius of Fireball";
            public override int Level=>11;
            public override void ApplyUpgrade(TowerModel diablo){
				ProjectileModel projectile=diablo.GetAbilities().First(a=>a.name.Contains("Fireball")).GetBehavior<ActivateAttackModel>().
					attacks[0].weapons[0].projectile;
				projectile.radius=10;
				projectile.GetBehavior<CreateProjectileOnContactModel>().projectile.radius=25;
            }
        }
        public class L12:ModHeroLevel<Diablo>{
            public override string Description=>"Halves the Terror required for Fireball";
            public override int Level=>12;
            public override void ApplyUpgrade(TowerModel diablo){
				//read upgrade override
            }
        }
        public class L13:ModHeroLevel<Diablo>{
            public override string Description=>"Further attack range";
            public override int Level=>13;
            public override void ApplyUpgrade(TowerModel diablo){
				diablo.range=32;
				diablo.GetAttackModel().range=diablo.range;
				diablo.GetAbilities().First(a=>a.name.Contains("PrimeEvil")).GetBehavior<ActivateAttackModel>().attacks.
					First(a=>a.name.Contains("Base")).range=32;
            }
        }
        public class L14:ModHeroLevel<Diablo>{
            public override string Description=>"At 70 Terror, Attacks set fire to bloons for a short time";
            public override int Level=>14;
            public override void ApplyUpgrade(TowerModel diablo){
            }
        }
        public class L15:ModHeroLevel<Diablo>{
            public override string Description=>"Holds 25 more Terror";
            public override int Level=>15;
            public override void ApplyUpgrade(TowerModel diablo){
				//read upgrade override
            }
        }
        public class L16:ModHeroLevel<Diablo>{
            public override string AbilityName=>"Terrorize";
            public override string AbilityDescription=>"Triples the amount of Terror generated from attacks and Fireball becomes automatically cast at no cost for a short time";
            public override string Description=>AbilityName+": "+AbilityDescription;
            public override int Level=>16;
            public override void ApplyUpgrade(TowerModel diablo){
				AbilityModel terrorize=BlankAbilityModel.Duplicate();
				terrorize.name="Terrorize";
				terrorize.icon=new(){guidRef="Ui[Diablo-TerrorizeIcon]"};
				terrorize.cooldown=45;
				terrorize.addedViaUpgrade="Diablo-Diablo Level 16";
				ActivateAttackModel fireball=diablo.GetAbilities().First(a=>a.name.Contains("Fireball")).GetBehavior<ActivateAttackModel>().Duplicate();
				fireball.lifespan=15;
				fireball.isOneShot=false;
				terrorize.AddBehavior(fireball);
				diablo.AddBehavior(terrorize);
            }
        }
        public class L17:ModHeroLevel<Diablo>{
            public override string Description=>"Increases attack and ability damage";
            public override int Level=>17;
            public override void ApplyUpgrade(TowerModel diablo){
				ProjectileModel proj=diablo.GetAttackModel().weapons[0].projectile;
				proj.GetDamageModel().damage*=2;
				proj.GetBehavior<CreateProjectileOnContactModel>().projectile.GetDamageModel().damage*=1.5f;
				ProjectileModel fireballProj=diablo.GetAbilities().First(a=>a.name.Contains("Fireball")).
					GetBehavior<ActivateAttackModel>().attacks[0].weapons[0].projectile;
				fireballProj.GetDamageModel().damage*=1.7f;
				fireballProj.GetBehavior<CreateProjectileOnContactModel>().projectile.GetDamageModel().damage*=1.5f;
				Il2CppReferenceArray<AttackModel>primeEvilAttacks=diablo.GetAbilities().First(a=>a.name.Contains("PrimeEvil"))
					.GetBehavior<ActivateAttackModel>().attacks;
				primeEvilAttacks[0].weapons[0].projectile.GetDamageModel().damage*=4;
				ProjectileModel primeEvilMelee=primeEvilAttacks[1].weapons[0].projectile;
				primeEvilMelee.GetDamageModel().damage*=2;
				primeEvilMelee.GetBehavior<CreateProjectileOnContactModel>().projectile.GetDamageModel().damage*=1.5f;
				ProjectileModel terrorizeFireball=diablo.GetAbilities().First(a=>a.name.Contains("Terrorize")).
					GetBehavior<ActivateAttackModel>().attacks[0].weapons[0].projectile;
				terrorizeFireball.GetDamageModel().damage*=1.7f;
				terrorizeFireball.GetBehavior<CreateProjectileOnContactModel>().projectile.GetDamageModel().damage*=1.5f;
            }
        }
        public class L18:ModHeroLevel<Diablo>{
            public override string Description=>"Faster attack speed";
            public override int Level=>18;
            public override void ApplyUpgrade(TowerModel diablo){
				diablo.GetAttackModel().weapons[0].rate=0.9f;
				diablo.GetAbilities().First(a=>a.name.Contains("PrimeEvil")).GetBehavior<ActivateAttackModel>().attacks[1].weapons[0].rate=0.9f;
            }
        }
        public class L19:ModHeroLevel<Diablo>{
            public override string Description=>"Terrorize lasts for longer";
            public override int Level=>19;
            public override void ApplyUpgrade(TowerModel diablo){
				diablo.GetAbilities().First(a=>a.name.Contains("Terrorize")).GetBehavior<ActivateAttackModel>().lifespan=25;
            }
        }
        public class L20:ModHeroLevel<Diablo>{
            public override string Description=>"Prime Evil lasts for twice as long and Terrorize can be cast while Prime Evil is active";
            public override int Level=>20;
            public override void ApplyUpgrade(TowerModel diablo){
				diablo.GetAbilities().First(a=>a.name.Contains("PrimeEvil")).GetBehavior<ActivateAttackModel>().lifespan*=2;
            }
        }
		public override bool Ability(string ability,Tower tower){
			DiabloBehaviour behaviour=tower.Node.graphic.GetComponent<DiabloBehaviour>();
			string activeName=behaviour.ActiveForm.name;
			//rather then do a contains like 10 times in the same method, store the result
			bool isBase=activeName.Contains("Base");
			switch(ability){
				case"AbilityModel_Fireball":
					if(behaviour.TerrorizeActive){
						return false;
					}
					if(behaviour.Terror>behaviour.FireballCost||NoTerrorCost){
						if(isBase){
							PlaySound("Diablo-BaseFireball");
						}else{
							PlaySound("Diablo-PrimeFireball");
						}
						behaviour.Terror-=behaviour.FireballCost;
						return true;
					}else{
						PlaySound("Diablo-NoTerror");
						return false;
					}
				case"AbilityModel_Terrorize":
					if(isBase||tower.towerModel.tier==20){
						if(isBase){
							PlaySound("Diablo-BaseTerrorize");
						}else{
							PlaySound("Diablo-PrimeTerrorize");
						}
						behaviour.TerrorizeActive=true;
						return true;
					}else{
						return false;
					}
				case"AbilityModel_PrimeEvil":
					if(isBase){
						if(behaviour.Terror>99||NoTerrorCost){
							PlaySound("Diablo-BasePrimeEvil");
							behaviour.Terror=0;
							return true;
						}else{
							PlaySound("Diablo-NoTerror");
							return false;
						}
					}else{
						return false;
					}
			}
			return true;
		}
		public override void Upgrade(string upgradeName,Tower tower){
			DiabloBehaviour behaviour=DiabloBehaviour.Instance;
			int tier=int.Parse(upgradeName.Split(' ').Last());
			switch(tier){
				case 4:
					behaviour.TerrorCap=100;
					break;
				case 12:
					behaviour.FireballCost=7.5f;
					break;
				case 15:
					behaviour.TerrorCap=125;
					break;
			}
			behaviour.PlayUpgradeSound();
			behaviour.Tier=tier;
		}
		public override void Create(Tower tower){
			PlaySound("Diablo-BaseBirth");
		}
		public override void Attack(Weapon weapon){
			DiabloBehaviour behaviour=DiabloBehaviour.Instance;
			bool isBase=behaviour.ActiveForm.name.Contains("Base");
			if(isBase){
				if(weapon.attack.attackModel.name.Contains("Base")&&behaviour.BaseWeapon==null){
					behaviour.BaseWeapon=weapon;
				}
				if(behaviour.Terror<behaviour.TerrorCap){
					if(behaviour.TerrorizeActive){
						behaviour.Terror+=6;
					}else{
						behaviour.Terror+=3;
					}
				}
			}
			Animator anim=behaviour.ActiveForm.GetComponent<Animator>();
			switch(weapon.attack.attackModel.name){
				case"AttackModel_BaseAttack":
					if(weapon.weaponModel.rate==0.9){
						PlayAnimation(anim,"FastAttack"+new System.Random().Next(1,4));
					}else{
						PlayAnimation(anim,"Attack"+new System.Random().Next(1,4));
					}
					break;
				case"AttackModel_FireballAttack":
					PlayAnimation(anim,"AbilityForward");
					break;
			}
		}
		public override void Select(Tower tower){
			DiabloBehaviour.Instance.PlaySelectSound();
		}
		[HarmonyPatch(typeof(Projectile),"Initialise")]
		public class ProjectileInitialise_Patch{
			[HarmonyPrefix]
			public static void Prefix(ref Model modelToUse){
				DiabloBehaviour behaviour=DiabloBehaviour.Instance;
				ProjectileModel projModel=modelToUse.Cast<ProjectileModel>();
				if(behaviour!=null){
					//Log(behaviour.Terror+" "+projModel.id+" "+projModel.HasBehavior<AddBehaviorToBloonModel>()+" "+behaviour.Tier);
					if(behaviour.Terror>59&&projModel.id.Contains("Diablo")&&!projModel.HasBehavior<AddBehaviorToBloonModel>()&&behaviour.Tier>13){
						projModel.AddBehavior(AddFire);
					}
				}
			}
		}
		/*[HarmonyPatch(typeof(AddBehaviorToBloon),"Initialise")]
		public class AddBehaviorToBloonModel_Patch{
			[HarmonyPrefix]
			public static void Prefix(){ 
				Log("addbehaviourinit");
			}
		}*/
		[HarmonyPatch(typeof(Bloon),"Damage")]
		public class BloonDamage_Patch{
			[HarmonyPrefix]
			public static void Prefix(ref Bloon __instance,Tower tower,ref float totalAmount){
				if(tower!=null){
					TowerModel towerModel=tower.towerModel;
					if(towerModel.baseId.Contains("Diablo")){
						DiabloBehaviour behaviour=DiabloBehaviour.Instance;
						if(behaviour.BonusDamageActive&&behaviour.Tier>7){
							totalAmount*=1.5f;
						}
						if(__instance.bloonModel.baseId=="Lead"&&behaviour.Tier>6){
							__instance.Destroy();
						}
					}
				}
			}
		}
		[HarmonyPatch(typeof(ActivateAttack),"Activate")]
		public class ActivateAttackActivate_Patch{
			[HarmonyPostfix]
			public static void Postfix(ref ActivateAttack __instance){
				Ability ability=__instance.entity.dependants.First(a=>a.GetIl2CppType().Name=="Ability").Cast<Ability>();
				Tower tower=ability.tower;
				if(tower.towerModel.baseId.Contains("Diablo")){
					DiabloBehaviour behaviour=tower.Node.graphic.GetComponent<DiabloBehaviour>();
					switch(ability.abilityModel.name){
						case"AbilityModel_PrimeEvil":
							MelonCoroutines.Start(behaviour.SwapForm());
							break;
						case"AbilityModel_Terrorize":
							behaviour.TerrorizeActive=true;
							break;
					}
				}
			}
		}
		[HarmonyPatch(typeof(ActivateAttack),"RemoveProcess")]
		public class ActivateAttackRemoveProcess_Patch{
			[HarmonyPostfix]
			public static void Postfix(ref ActivateAttack __instance){
				Ability ability=__instance.entity.dependants.First(a=>a.GetIl2CppType().Name=="Ability").Cast<Ability>();
				Tower tower=ability.tower;
				if(tower.towerModel.baseId.Contains("Diablo")){
					DiabloBehaviour behaviour=tower.Node.graphic.GetComponent<DiabloBehaviour>();
					switch(ability.abilityModel.name){
						case"AbilityModel_PrimeEvil":
							MelonCoroutines.Start(behaviour.SwapForm());
							break;
						case"AbilityModel_Terrorize":
							behaviour.TerrorizeActive=false;
							break;
					}
				}
			}
		}
		public override void RoundStart(){
			DiabloBehaviour behaviour=DiabloBehaviour.Instance;
			if(behaviour!=null){
				PlayAnimation(behaviour.ActiveForm.GetComponent<Animator>(),"StandReady");
			}
		}
		public override void RoundEnd(){
			DiabloBehaviour behaviour=DiabloBehaviour.Instance;
			if(behaviour!=null){
				PlayAnimation(behaviour.ActiveForm.GetComponent<Animator>(),"StandReadyEnd");
			}
		}
    }
}