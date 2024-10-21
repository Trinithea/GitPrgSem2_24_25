partial class Program
{
    abstract class Creature(string Name, float MaxHealth)
    {
        protected struct Multiplier(float value, int turnsRemaining)
        {
            public float Value { get; } = value;
            public int TurnsRemaining { get; set; } = turnsRemaining;

            public void EndOfTurn(){
                TurnsRemaining--;
            }
        }

        protected Random rng = new();
        public float MaxHealth { get; } = MaxHealth;
        public float Health { get; protected set; } = MaxHealth;
        public string Name { get; } = Name;

        public bool IsOnCooldown {
            get {
                return coolDown > 0;
            }
        }

        protected List<Multiplier> DefenseMultipliers = [];
        protected List<Multiplier> StrengthMultipliers = [];

        private int coolDown = 0;

        public override string ToString()
        {
            return  $"{Name} the {GetType().Name} - Health: {Health}{(coolDown == 0 ? "" : $" Cooldown: {coolDown}")}";
        }

        protected virtual void TakeDamage(float Damage, Creature Attacker)
        {
            float mult = DefenseMultipliers.Count <= 0 ? 1 : DefenseMultipliers.Select(item => item.Value).Aggregate((product, current) => product * current);
            
            Health -= Damage * mult;
        }

        public void Heal(float health){
            Health = Math.Min(Health + health, MaxHealth);
        }

        public virtual void DealDamage(float Damage, Creature Enemy){
            float mult = StrengthMultipliers.Count <= 0 ? 1 : StrengthMultipliers.Select(item => item.Value).Aggregate((product, current) => product * current);

            Enemy.TakeDamage(Damage*mult, this);
        }

        public virtual string CombatMove(List<Creature> EnemyParty, List<Creature> Allies){
            string output = GenerateMove(EnemyParty, Allies);
            
            DefenseMultipliers.ForEach(item => item.EndOfTurn());
            StrengthMultipliers.ForEach(item => item.EndOfTurn());

            DefenseMultipliers.RemoveAll(item => item.TurnsRemaining <= 0);
            StrengthMultipliers.RemoveAll(item => item.TurnsRemaining <= 0);

            return output;
        }

        public void DecreaseCooldown(){
            coolDown--;
        }
        public void ResetCooldown(){
            coolDown = 0;
        }

        protected abstract string GenerateMove(List<Creature> EnemyParty, List<Creature> Allies);

        public virtual void Stun(int stun){
            coolDown = Math.Max(coolDown, stun);
        }
        
        public void AddDefence(float mult, int moves){ 
            DefenseMultipliers.Add(new(mult, moves));   //lower that 1 increases defense
        }

        public void AddStrength(float mult, int moves){ //higher that 1 increases attack
            StrengthMultipliers.Add(new(mult, moves));
        }
    }

    class Soldier(string Name) : Creature(Name, 250)
    {
        enum SoldierMove{
            Light_Attackđ10Đ,
            Heavy_Attackđ30Đ,
            DefendđReduce_50ßĐ,
        }

        protected override string GenerateMove(List<Creature> enemyParty, List<Creature> _)
        {
            SoldierMove decision = MakeDecision<SoldierMove>($"{ToString()} is playing\nChoose an action:");
            int targetedEnemy;

            switch (decision) {
                case SoldierMove.Light_Attackđ10Đ:
                    targetedEnemy = MakeDecision("Attack which enemy: ", enemyParty.Select(enemy => enemy.ToString()).ToList());
                    DealDamage(10, enemyParty[targetedEnemy]);

                    return "Light Attack";                    

                case SoldierMove.Heavy_Attackđ30Đ:
                    targetedEnemy = MakeDecision("Attack which enemy: ", enemyParty.Select(enemy => enemy.ToString()).ToList());
                    DealDamage(30, enemyParty[targetedEnemy]);

                    Stun(2);

                    return "Heavy Attack";

                case SoldierMove.DefendđReduce_50ßĐ:
                    DefenseMultipliers.Add(new(0.5f, 1));

                    return "Defend";
            }

            throw new Exception("What are you doing here?");
        }
    }
    class Healer(string Name) : Creature(Name, 100)
    {
        enum HealerMove{
            Light_Attackđ5Đ,
            Healđ22Đ,
            Heal_Partyđ5Đ,
        }

        protected override string GenerateMove(List<Creature> enemyParty, List<Creature> SoldierParty)
        {
            HealerMove decision = MakeDecision<HealerMove>($"{ToString()} is playing\nChoose an action:");
            int targetedEnemy;
            int targetedSoldier;

            switch (decision) {
                case HealerMove.Light_Attackđ5Đ:
                    targetedEnemy = MakeDecision("Attack which enemy: ", enemyParty.Select(enemy => enemy.ToString()).ToList());
                    DealDamage(5, enemyParty[targetedEnemy]);

                    return "Light Attack";

                case HealerMove.Healđ22Đ:
                    targetedSoldier = MakeDecision("Heal which hero: ", SoldierParty.Select(enemy => enemy.ToString()).ToList());
                    
                    SoldierParty[targetedSoldier].Heal(30);

                    return "Heal Ally";

                case HealerMove.Heal_Partyđ5Đ:
                    SoldierParty.ForEach(cr => cr.Heal(10));
                    Stun(1);
                    return "Heal Party";
            }

            throw new Exception("What are you doing here?");
        }
    }
    class Gambler(string Name) : Creature(Name, 151)
    {
        enum GamblerMove{
            Small_Betđ5Đ,
            All_On_Redđ20Đ,
            All_On_Blackđ50ßĐ,
            // multiply random creature damage by 20
            Not_Today,
        }

        protected override string GenerateMove(List<Creature> enemyParty, List<Creature> HeroParty)
        {
            GamblerMove decision = MakeDecision<GamblerMove>($"{ToString()} is playing\nChoose an action:");
            int targetedEnemy;
            List<Creature> targetedParty;

            switch (decision) {
                case GamblerMove.Small_Betđ5Đ:
                    targetedEnemy = MakeDecision("Attack which enemy: ", enemyParty.Select(enemy => enemy.ToString()).ToList());
                    DealDamage(rng.Next(5,10), enemyParty[targetedEnemy]);

                    return "Small Bet";

                case GamblerMove.All_On_Redđ20Đ:
                    targetedParty = rng.Next(1) == 0 ? enemyParty : HeroParty;
                    targetedEnemy = rng.Next(targetedParty.Count);

                    targetedParty[targetedEnemy].Heal(targetedParty[targetedEnemy].MaxHealth);

                    return "All On Red";

                case GamblerMove.All_On_Blackđ50ßĐ:
                    targetedEnemy = MakeDecision("Attack which enemy: ", enemyParty.Select(enemy => enemy.ToString()).ToList());

                    //GL
                    if (rng.Next(1) == 0) DealDamage(float.MaxValue, enemyParty[targetedEnemy]);
                    else Health = 0;
                    

                    return "All On Black";

                case GamblerMove.Not_Today:
                    if (Health <= 1)
                    {
                        targetedEnemy = MakeDecision("Attack which enemy: ", enemyParty.Select(enemy => enemy.ToString()).ToList());
                        DealDamage(float.MaxValue, enemyParty[targetedEnemy]);
                        
                        return "DIE B*TCH";
                    }
                    return "You are a LOSER";
            }

            throw new Exception("What are you doing here?");
        }
    }
    class Arcanist(string Name) : Creature(Name, 140)
    {
        enum ArcanistMove{
            Add_Defenceđ20ßĐ,
            Add_Strenghtđ20ßĐ,
            Kickđ3Đ
        }

        protected override string GenerateMove(List<Creature> enemyParty, List<Creature> heroParty)
        {
            ArcanistMove decision = MakeDecision<ArcanistMove>($"{ToString()} is playing\nChoose an action:");
            int targetedEnemy;
            int targetedHero;

            switch (decision) {
                case ArcanistMove.Add_Defenceđ20ßĐ:
                    targetedHero = MakeDecision("Defend which hero: ", heroParty.Select(hero => hero.ToString()).ToList());
                    heroParty[targetedHero].AddDefence(0.8f, 3);
                    Stun(1);

                    return "Add Defence";                    

                case ArcanistMove.Add_Strenghtđ20ßĐ:
                    targetedHero = MakeDecision("Buff which hero: ", heroParty.Select(hero => hero.ToString()).ToList());
                    heroParty[targetedHero].AddStrength(1.2f, 5);
                    Stun(1);

                    return "Add Strength";
                case ArcanistMove.Kickđ3Đ:
                    targetedEnemy = MakeDecision("Attack which enemy: ", enemyParty.Select(enemy => enemy.ToString()).ToList());
                    DealDamage(3, enemyParty[targetedEnemy]);

                    return "Kick";
            }

            throw new Exception("What are you doing here?");
        }
    }
    class Stunner(string Name) : Creature(Name, 120)
    {
        enum StunnerMove{
            Stunđ2Đ,
            Light_Attackđ20Đ,
            Attack_Allđ10Đ

        }

        protected override string GenerateMove(List<Creature> enemyParty, List<Creature> heroParty)
        {
            StunnerMove decision = MakeDecision<StunnerMove>($"{ToString()} is playing\nChoose an action:");
            int targetedEnemy;

            switch (decision) {
                case StunnerMove.Stunđ2Đ:
                    targetedEnemy = MakeDecision("Stun which enemy: ", enemyParty.Select(enemy => enemy.ToString()).ToList());
                    
                    if (rng.Next(3)!=0) enemyParty[targetedEnemy].Stun(2);
                    else Stun(2);
                    
                    return "Stun";                    
                case StunnerMove.Light_Attackđ20Đ:
                    targetedEnemy = MakeDecision("Attack which enemy: ", enemyParty.Select(enemy => enemy.ToString()).ToList());
                    DealDamage(20, enemyParty[targetedEnemy]);
                    return "Light Attack";
                case StunnerMove.Attack_Allđ10Đ:
                    enemyParty.ForEach(cr => DealDamage(10, cr));
                    return "Attack All";
                    
            }

            throw new Exception("What are you doing here?");
        }
    }




    class Goblin(string Name) : Creature(Name, 35)
    {
        protected override string GenerateMove(List<Creature> enemyParty, List<Creature> _)
        {

            Creature enemy = enemyParty.MinBy(static enemy => enemy is null ? 0 : enemy.Health) ?? throw new Exception("Fuck");
            DealDamage(15, enemy);

            return "Bite";
        }
    }
    class Spider(string Name) : Creature(Name, 41)
    {
        protected override string GenerateMove(List<Creature> enemyParty, List<Creature> _)
        {
            Creature? enemy = enemyParty[rng.Next(enemyParty.Count)];

            DealDamage(9, enemy);

            if(rng.Next(4) == 0) { enemy?.Stun(2);}
            return "Web";
        }
    }
    class Succubus(string Name) : Creature(Name, 50)
    {
        protected override string GenerateMove(List<Creature> enemyParty, List<Creature> allyParty)
        {
            Creature? ally = allyParty[rng.Next(allyParty.Count)];
            int move = rng.Next(3);
                
            
            if (move == 0) {
                ally.AddDefence(0.7f, 2);
                return "Kiss";
            }

            if (move == 1)
            {
                ally.AddStrength(1.5f, 3);
                return "Suck";
            }
            return "Nothing cos' she was bussy";
        }
    }
    class Orc(string Name) : Creature(Name, 50)
    {
        protected override string GenerateMove(List<Creature> enemyParty, List<Creature> _)
        {
            Creature? enemy = enemyParty[rng.Next(enemyParty.Count)];
            DealDamage(30, enemy);

            return "Slash";
        }
    }
    class Beholder(string Name) : Creature(Name, 1000)
    {
        bool phase = false;
        protected override string GenerateMove(List<Creature> enemyParty, List<Creature> allyParty)
        {
            if (Health <= MaxHealth/2 && !phase)
            {
                for (int i = 0; i < enemyParty.Count; i++)
                {
                    if (allyParty[i].GetType().Name != "Beholder") DealDamage(float.MaxValue, allyParty[i]);
                    StrengthMultipliers.Add(new(2.5f, int.MaxValue));
                }
                phase = true;
            }
            Creature? enemy = enemyParty[rng.Next(enemyParty.Count)];
            DealDamage(70, enemy);
            return phase == false ? "Behold": "Behold Serius";
        }
    }
    
}