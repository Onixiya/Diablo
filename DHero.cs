namespace Diablo{
    public abstract class DHero:ModHero{
        public AssetBundle LoadedBundle=null;
        public virtual void Attack(Weapon weapon){}
        public virtual void Upgrade(string upgradeName,Tower tower){}
        public virtual bool Ability(string ability,Tower tower){
			return true;
		}
        public virtual void Create(Tower tower){}
        public virtual void Select(Tower tower){}
        public virtual void Sell(Tower tower){}
        public virtual void RoundStart(){}
        public virtual void RoundEnd(){}
        public virtual Dictionary<string,Il2CppSystem.Type>Behaviours=>new();
    }
}
