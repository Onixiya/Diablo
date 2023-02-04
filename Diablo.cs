using Il2CppInterop.Runtime.InteropTypes.Arrays;

namespace Diablo{
    public class Diablo:DHero{
		public static AddBehaviorToBloonModel FireDOT=null;
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
			diablo.GetAttackModel().name="BaseAttack";
			diablo.GetAttackModel().weapons[0].rate=1.25f;
			diablo.GetAttackModel().weapons[0].projectile.GetDamageModel().damage=2;
			diablo.GetAttackModel().weapons[0].projectile.pierce=5;
			diablo.GetAttackModel().weapons[0].projectile.radius=15;
			diablo.GetAttackModel().weapons[0].projectile.AddBehavior(new CreateProjectileOnContactModel("CPOCM",
				diablo.GetAttackModel().weapons[0].projectile.GetBehavior<CreateProjectileOnExhaustFractionModel>().projectile.Duplicate(),
				diablo.GetAttackModel().weapons[0].projectile.GetBehavior<CreateProjectileOnExhaustFractionModel>().emission.Duplicate(),
				true,false,false));
			diablo.GetAttackModel().weapons[0].projectile.GetBehavior<CreateProjectileOnContactModel>().projectile.pierce=5;
			diablo.GetAttackModel().weapons[0].projectile.GetBehavior<CreateProjectileOnContactModel>().projectile.radius=15;
			diablo.GetAttackModel().weapons[0].projectile.GetBehavior<CreateProjectileOnContactModel>().projectile.GetDamageModel().damage=2;
			diablo.GetAttackModel().weapons[0].projectile.GetBehavior<CreateProjectileOnContactModel>().projectile.RemoveBehavior<CreateEffectOnExhaustFractionModel>();
			diablo.GetAttackModel().weapons[0].projectile.RemoveBehavior<CreateProjectileOnExhaustFractionModel>();
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
				diablo.GetAttackModel().weapons[0].projectile.GetDamageModel().damage=4;
				diablo.GetAttackModel().weapons[0].projectile.GetBehavior<CreateProjectileOnContactModel>().projectile.GetDamageModel().damage=4;
            }
        }
        public class L4:ModHeroLevel<Diablo>{
            public override string Description=>"Starts gathering Terror from attacked bloons, gaining small buffs with each point of Terror";
            public override int Level=>4;
            public override void ApplyUpgrade(TowerModel diablo){
            }
        }
        public class L5:ModHeroLevel<Diablo>{
            public override string Description=>"Hits more bloons at once";
            public override int Level=>5;
            public override void ApplyUpgrade(TowerModel diablo){
				diablo.GetAttackModel().weapons[0].projectile.radius=20;
				diablo.GetAttackModel().weapons[0].projectile.GetBehavior<CreateProjectileOnContactModel>().projectile.pierce=20;
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
				diablo.GetAttackModel().weapons[0].projectile.GetDamageModel().damage*=1.5f;
				diablo.GetAttackModel().weapons[0].projectile.GetBehavior<CreateProjectileOnContactModel>().projectile.GetDamageModel().damage*=1.5f;
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
				ActivateAttackModel fireBallAttack=Game.instance.model.GetTowerFromId("BombShooter-040").GetAbility().
					GetBehavior<ActivateAttackModel>().Duplicate();
				fireBallAttack.attacks[0]=Game.instance.model.GetTowerFromId("TackShooter-500").GetAttackModels().
					First(a=>a.name.Contains("Meteor")).Duplicate();
				fireBallAttack.attacks[0].name="FireballAttack";
				fireBallAttack.attacks[0].AddBehavior(diablo.GetAttackModel().GetBehavior<RotateToTargetModel>().Duplicate());
				FireDOT=fireBallAttack.attacks[0].weapons[0].projectile.GetBehavior<CreateProjectileOnContactModel>().projectile.
					GetBehavior<AddBehaviorToBloonModel>().Duplicate();
				FireDOT.collisionPass=0;
				fireBallAttack.attacks[0].weapons[0].projectile.GetBehavior<CreateProjectileOnContactModel>().projectile.
					RemoveBehavior<AddBehaviorToBloonModel>();
				fireBallAttack.attacks[0].weapons[0].rate=3;
				fireBallAttack.attacks[0].weapons[0].projectile.GetDamageModel().damage=50;
				fireBallAttack.attacks[0].weapons[0].projectile.GetBehavior<CreateProjectileOnContactModel>().projectile.GetDamageModel().damage=25;
				fireBall.AddBehavior(fireBallAttack);
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
				attackFire.GetBehavior<AttackFilterModel>().filters.AddItem(new FilterWithTagModel("FWTM","Moabs",true));
				attackMelee.weapons[0].projectile.GetDamageModel().damage=20;
				attackMelee.weapons[0].projectile.GetBehavior<CreateProjectileOnContactModel>().projectile.GetDamageModel().damage=20;
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
            public override string Description=>"Attacks and abilities now set fire to bloons for a short time";
            public override int Level=>14;
            public override void ApplyUpgrade(TowerModel diablo){
				ProjectileModel melee=diablo.GetAttackModel().weapons[0].projectile;
				melee.GetBehavior<CreateProjectileOnContactModel>().projectile.AddBehavior(FireDOT.Duplicate());
				melee.AddBehavior(FireDOT.Duplicate());
				diablo.GetAbilities().First(a=>a.name.Contains("Fireball")).GetBehavior<ActivateAttackModel>().attacks[0].
					weapons[0].projectile.AddBehavior(FireDOT.Duplicate());
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
			switch(ability){
				case"AbilityModel_Fireball":
					if(behaviour.TerrorizeActive){
						return false;
					}
					if(behaviour.Terror>behaviour.FireballCost||NoTerrorCost){
						if(behaviour.ActiveForm.name.Contains("Base")){
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
					if(behaviour.ActiveForm.name.Contains("Base")||tower.towerModel.tier==20){
						if(behaviour.ActiveForm.name.Contains("Base")){
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
					if(behaviour.ActiveForm.name.Contains("Base")){
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
			DiabloBehaviour behaviour=tower.Node.graphic.GetComponent<DiabloBehaviour>();
			switch(int.Parse(upgradeName.Last().ToString())){
				case 4:
					behaviour.TerrorCap=100;
					break;
				case 12:
					behaviour.FireballCost=5;
					break;
				case 15:
					behaviour.TerrorCap=125;
					break;
			}
			tower.Node.graphic.GetComponent<DiabloBehaviour>().PlayUpgradeSound();
		}
		public override void Create(Tower tower){
			PlaySound("Diablo-BaseBirth");
		}
		public override void Attack(Weapon weapon){
			DiabloBehaviour behaviour=weapon.attack.tower.Node.graphic.GetComponent<DiabloBehaviour>();
			if(behaviour.ActiveForm.name.Contains("Base")){
				if(weapon.attack.attackModel.name.Contains("Base")&&behaviour.baseWeapon==null){
					behaviour.baseWeapon=weapon;
				}
				if(behaviour.Terror<behaviour.TerrorCap){
					if(behaviour.TerrorizeActive){
						behaviour.Terror+=3;
					}else{
						behaviour.Terror+=1;
					}
				}
				if(behaviour.BonusDamageActive){
					//what the fuck is the difference between newprojectiles and newprojectiles2
					for(int i=0;i<weapon.newProjectiles.list.Count;i++){
						weapon.newProjectiles[i].GetProjectileBehavior<Damage>().damageModel.damage*=1.4f;
					}
					for(int i=0;i<weapon.newProjectiles2.list.Count;i++){
						weapon.newProjectiles2[i].GetProjectileBehavior<Damage>().damageModel.damage*=1.4f;
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
			tower.Node.graphic.GetComponent<DiabloBehaviour>().PlaySelectSound();
		}
		[HarmonyPatch(typeof(Bloon),"Damage")]
		public class BloonDamage_Patch{
			[HarmonyPrefix]
			public static void Prefix(ref Bloon __instance,Tower tower){
				TowerModel towerModel=tower.towerModel;
				if(__instance.bloonModel.baseId=="Lead"&&towerModel.baseId.Contains("Diablo")&&towerModel.tier>6){
					__instance.Destroy();
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
    }
}