using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L45_gladiatorFights
{
    internal class Program
    {
        static void Main(string[] args)
        {
            const int CommantMage = 1;
            const int CommantWarrior = 2;
            const int CommantBarbarian = 3;
            const int CommantPaladin = 4;
            const int CommantArcher = 5;
            const int CommantExit = 6;

            List<Fighter> fighters = new List<Fighter>();
            int menuNumber;
            int delimeterLenght = 50;
            char delimeter = '=';
            bool isOpen = true;

            while (isOpen)
            {
                Console.Clear();
                Console.WriteLine($"Выберите 2х бойцов для сражения на арене.\nПосле выбора бойцов бой " +
                                  $"начнется автоматически.\n" + new string(delimeter, delimeterLenght));
                Console.WriteLine($"{CommantMage} - Выбрать мага.\n{CommantWarrior} - Выбрать война.\n" +
                                  $"{CommantBarbarian} - Выбрать варвара.\n{CommantPaladin} - Выбрать " +
                                  $"паладина.\n{CommantArcher} - Выбрать лучника.\n{CommantExit} - Выйти" +
                                  $" из программы.\n" + new string(delimeter, delimeterLenght));
                Console.WriteLine($"Выбрано бойцов: {fighters.Count}\n" + new string(delimeter, delimeterLenght));

                Console.Write("Выберите пункт меню: ");

                if (int.TryParse(Console.ReadLine(), out menuNumber))
                {
                    Console.Clear();

                    switch (menuNumber)
                    {
                        case CommantWarrior:

                        case CommantMage:

                        case CommantBarbarian:

                        case CommantPaladin:

                        case CommantArcher:
                            fighters.Add(CreateFighters((TypeFighters)menuNumber));
                            break;

                        case CommantExit:
                            isOpen = false;
                            continue;

                        default:
                            ShowError();
                            break;
                    }
                }
                else
                {
                    ShowError();
                }

                Console.WriteLine("Для возврашения в меню, нажмите любую клавишу...");
                Console.ReadKey(true);

                if (fighters.Count > 1)
                {
                    int numberOfRound = 1;

                    Console.Clear();

                    while (isOpen)
                    {
                        Console.WriteLine(new string(delimeter, delimeterLenght) + $"\nРаунд №{numberOfRound}.");
                        Console.WriteLine($"{fighters[0].TypeFighters} - Здоровье: {fighters[0].CurrentHelth}\t" +
                                          $"{fighters[1].TypeFighters} - Здоровье: {fighters[1].CurrentHelth}");

                        fighters[0].Attack(fighters[1]);
                        fighters[1].Attack(fighters[0]);

                        numberOfRound++;

                        if (fighters[0].CurrentHelth <= 0 || fighters[1].CurrentHelth <= 0)
                        {
                            isOpen = false;

                            Console.WriteLine(new string(delimeter, delimeterLenght));

                            if (fighters[0].CurrentHelth <= 0 && fighters[1].CurrentHelth <= 0)
                                Console.WriteLine("Ничья!! Противника пали одновременно.");
                            else if (fighters[0].CurrentHelth <= 0)
                                Console.WriteLine($"Победил {fighters[1].TypeFighters}.");
                            else
                                Console.WriteLine($"Победил {fighters[0].TypeFighters}.");

                            Console.WriteLine(new string(delimeter, delimeterLenght));
                        }

                        Console.ReadKey(true);
                    }
                }
            }
        }

        static void ShowError()
        {
            Console.WriteLine("\nВы ввели некорректное значение.");
        }

        static Fighter CreateFighters(TypeFighters type)
        {
            Console.Write("Введите имя бойца: ");
            string name = Console.ReadLine();

            switch (type)
            {
                case TypeFighters.Mage:
                    return new Mage(name, type, 200, 35, 0, 40);

                case TypeFighters.Warior:
                    return new Warior(name, type, 350, 15, 15);

                case TypeFighters.Barbarian:
                    return new Barbarian(name, type, 400, 25, 7, 30);

                case TypeFighters.Paladin:
                    return new Paladin(name, type, 300, 20, 10, 35);

                case TypeFighters.Archer:
                    return new Archer(name, type, 275, 27, 5, 25);
                default:
                    return null;
            }
        }
    }

    class Fighter
    {
        protected TypeFighters _type;
        protected string _name;
        protected int _currentHelth;
        protected int _maxHealth;
        protected int _damage;
        protected int _armor;

        public Fighter(string name, TypeFighters type, int helthPoint, int damage, int armor)
        {
            _type = type;
            _name = name;
            _currentHelth = helthPoint;
            _maxHealth = helthPoint;
            _damage = damage;
            _armor = armor;
        }

        public TypeFighters TypeFighters
        {
            get
            {
                return _type;
            }
        }

        public int CurrentHelth
        {
            get
            {
                return _currentHelth;
            }
        }

        virtual public void SetDamage(int damage)
        {
            int calculateDamage = damage - _armor;
            calculateDamage = (calculateDamage < 0 ? 0 : calculateDamage);
            _currentHelth -= calculateDamage;
            Console.WriteLine($"получает: {calculateDamage} ед. урона.");

            if (_currentHelth < 0)
                _currentHelth = 0;
        }

        virtual public void Attack(Fighter enemy)
        {
            Console.Write($"{_type} - Атакует.\t{enemy.TypeFighters} - ");
        }
    }

    class Mage : Fighter
    {
        private int _manaPoint;
        private int _shieldPoint;
        private Skill _skill = new Skill("Energy Shield", 10, 30);

        public Mage(string name, TypeFighters type, int helthPoint, int damage, int armor, int manaPoint) : base(name, type, helthPoint, damage, armor)
        {
            _manaPoint = manaPoint;
        }

        public override void SetDamage(int damage)
        {
            int remainingDamage = damage - _shieldPoint;
            _shieldPoint -= damage;

            if (remainingDamage > 0)
            {
                base.SetDamage(remainingDamage);
            }
            else
            {
                Console.WriteLine($"поглощает энерго-щитом урон, у щита остается {_shieldPoint} ед. прочности.");
            }

            if (_shieldPoint <= 0)
            {
                _skill.OnActive = false;
                _shieldPoint = 0;
            }
        }

        override public void Attack(Fighter enemy)
        {
            if (_manaPoint >= _skill.Cost && _shieldPoint <= 0 && _skill.OnActive == false)
            {
                _shieldPoint = _skill.Power;
                _manaPoint -= _skill.Cost;
                _skill.OnActive = true;
                Console.WriteLine($"Маг кастует на себя {_skill.Name} на {_skill.Power} едениц.");
            }
            else
            {
                base.Attack(enemy);
                enemy.SetDamage(_damage);
            }
        }
    }

    class Warior : Fighter
    {
        private int _timeSkill;
        private Skill _skill = new Skill("Fortyfy", 3, 20);

        public Warior(string name, TypeFighters type, int helthPoint, int damage, int armor) : base(name, type, helthPoint, damage, armor) { }

        public override void SetDamage(int damage)
        {
            base.SetDamage(damage);
        }

        override public void Attack(Fighter enemy)
        {
            if (_timeSkill > 0)
            {
                _timeSkill--;
            }
            else
            {
                _skill.OnActive = false;
                _armor -= _skill.Power;
            }

            if (_timeSkill <= 0 && _skill.OnActive == false)
            {
                _timeSkill = _skill.Cost;
                _armor += _skill.Power;
                _skill.OnActive = true;
                Console.WriteLine($"Боец использует {_skill.Name} и увеличивает защиту на {_skill.Power} едениц.");
            }
            else
            {
                base.Attack(enemy);
                enemy.SetDamage(_damage);
            }
        }
    }

    class Barbarian : Fighter
    {
        private int _rage;
        private int _percentageDamageAbsorbed;
        private Skill _skill = new Skill("Rage", 10, 25);

        public Barbarian(string name, TypeFighters type, int helthPoint, int damage, int armor, int percentageDamageAbsorbed) : base(name, type, helthPoint, damage, armor)
        {
            _percentageDamageAbsorbed = percentageDamageAbsorbed;
        }

        public override void SetDamage(int damage)
        {
            int damageAbsorbed = (int)(_percentageDamageAbsorbed * ((float)damage / 100));
            _rage += damageAbsorbed;
            base.SetDamage(damage - damageAbsorbed);
        }

        override public void Attack(Fighter enemy)
        {
            if (_rage >= _skill.Cost)
            {
                _armor -= _skill.Cost;
                _damage += _skill.Power;
                _rage -= _skill.Cost;
                _skill.OnActive = true;
                Console.WriteLine($"{_type} использует {_skill.Name} и усиливает следующий удар на {_skill.Power} едениц.");
            }

            if (_skill.OnActive)
            {
                base.Attack(enemy);
                enemy.SetDamage(_damage);
                _armor += _skill.Cost;
                _damage -= _skill.Power;
                _skill.OnActive = false;
            }
            else
            {
                base.Attack(enemy);
                enemy.SetDamage(_damage);
            }
        }
    }

    class Paladin : Fighter
    {
        private int _faith;
        private Skill _skill = new Skill("Heal", 10, 50);

        public Paladin(string name, TypeFighters type, int helthPoint, int damage, int armor, int faith) : base(name, type, helthPoint, damage, armor)
        {
            _faith = faith;
        }

        public override void SetDamage(int damage)
        {
            base.SetDamage(damage);
        }

        override public void Attack(Fighter enemy)
        {
            if (_faith >= _skill.Cost && _currentHelth < (_maxHealth / 2))
            {
                _currentHelth += _skill.Power;
                _faith -= _skill.Cost;
                Console.WriteLine($"Паладин использует {_skill.Name} и лечит себя на {_skill.Power} едениц.");
            }
            else
            {
                base.Attack(enemy);
                enemy.SetDamage(_damage);
            }
        }
    }

    class Archer : Fighter
    {
        private Random _random = new Random();
        private int _dodgeChance;
        private int _ammunition;

        public Archer(string name, TypeFighters type, int helthPoint, int damage, int armor, int dodgeChance) : base(name, type, helthPoint, damage, armor)
        {
            _dodgeChance = dodgeChance;
            _ammunition = _random.Next(30, 100);
        }

        public override void SetDamage(int damage)
        {
            if (_random.Next(100) > _dodgeChance)
                base.SetDamage(damage);
            else
                Console.WriteLine("Лучник увернулся от удара");
        }

        override public void Attack(Fighter enemy)
        {

            if (_ammunition <= 0)
            {
                _dodgeChance = 0;
                _damage /= 2;
            }

            base.Attack(enemy);
            enemy.SetDamage(_damage);
        }
    }

    class Skill
    {
        public Skill(string nameSkill, int costSkill, int powerSkill)
        {
            Name = nameSkill;
            Cost = costSkill;
            Power = powerSkill;
            OnActive = false;
        }

        public string Name { get; private set; }

        public int Cost { get; private set; }

        public int Power { get; private set; }

        public bool OnActive { get; set; }
    }

    enum TypeFighters
    {
        Mage = 1,
        Warior = 2,
        Barbarian = 3,
        Paladin = 4,
        Archer = 5,
    }
}
