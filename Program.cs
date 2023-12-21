using System;
using System.Collections.Generic;

namespace L45_gladiatorFights
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Arena arena = new Arena();

            arena.Run();
        }
    }

    static class Error
    {
        public static void Show()
        {
            Console.WriteLine("\nВы ввели некорректное значение.");
        }
    }

    static class FormatOutput
    {
        static FormatOutput()
        {
            DelimeterLenght = 50;
            Delimeter = '=';
        }

        public static int DelimeterLenght { get; private set; }
        public static char Delimeter { get; private set; }
    }

    class Arena
    {
        private const int CommantSelectMage = (int)TypeFighters.Mage;
        private const int CommantSelectWarrior = (int)TypeFighters.Warior;
        private const int CommantSelectBarbarian = (int)TypeFighters.Barbarian;
        private const int CommantSelectPaladin = (int)TypeFighters.Paladin;
        private const int CommantSelectArcher = (int)TypeFighters.Archer;
        private const int CommandStartFight = 6;
        private const int CommantExit = 7;

        List<Fighter> _fighters = new List<Fighter>();

        public void Run()
        {
            bool isOpen = true;

            while (isOpen)
            {
                ShowMenu();
                Console.Write("Выберите пункт меню: ");

                if (int.TryParse(Console.ReadLine(), out int menuNumber))
                {
                    Console.Clear();

                    switch (menuNumber)
                    {
                        case CommantSelectMage:
                            SelectFighter(TypeFighters.Mage);
                            break;

                        case CommantSelectWarrior:
                            SelectFighter(TypeFighters.Warior);
                            break;

                        case CommantSelectBarbarian:
                            SelectFighter(TypeFighters.Barbarian);
                            break;

                        case CommantSelectPaladin:
                            SelectFighter(TypeFighters.Paladin);
                            break;

                        case CommantSelectArcher:
                            SelectFighter(TypeFighters.Archer);
                            break;

                        case CommandStartFight:
                            Fight();
                            break;

                        case CommantExit:
                            isOpen = false;
                            continue;

                        default:
                            Error.Show();
                            break;
                    }
                }
                else
                {
                    Error.Show();
                }

                Console.WriteLine("Для возврашения в меню, нажмите любую клавишу...");
                Console.ReadKey(true);
            }
        }

        private void SelectFighter(TypeFighters type)
        {
            if (_fighters.Count < 2)
            {
                _fighters.Add(CreateFighter(type));
            }
            else if (_fighters.Count > 1)
            {
                Console.Clear();
                Console.WriteLine($"Какого бойца вы хотите заменить?\n1 - {_fighters[0].TypeFighters}\n2 - {_fighters[1].TypeFighters}");

                if (int.TryParse(Console.ReadLine(), out int numFighter))
                {
                    if (numFighter < 3 && numFighter > 0)
                    {
                        numFighter--;
                        _fighters[numFighter] = CreateFighter(type);
                    }
                    else
                    {
                        Error.Show();
                    }
                }
                else
                {
                    Error.Show();
                }
            }
        }

        private Fighter CreateFighter(TypeFighters type)
        {
            switch (type)
            {
                case TypeFighters.Mage:
                    return new Warior(type, 350, 15, 15);

                case TypeFighters.Warior:
                    return new Mage(type, 200, 35, 0, 40);

                case TypeFighters.Barbarian:
                    return new Barbarian(type, 400, 25, 7, 30);

                case TypeFighters.Paladin:
                    return new Paladin(type, 300, 20, 10, 35);

                case TypeFighters.Archer:
                    return new Archer(type, 275, 27, 5, 25);

                default:
                    return null;
            }
        }

        private void Fight()
        {
            if (_fighters.Count == 2)
            {
                int numberOfRound = 1;
                bool isFight = true;

                Console.Clear();

                while (isFight)
                {
                    Console.WriteLine(new string(FormatOutput.Delimeter, FormatOutput.DelimeterLenght) + $"\nРаунд №{numberOfRound}.");
                    Console.WriteLine($"{_fighters[0].TypeFighters} - Здоровье: {_fighters[0].CurrentHelth}\t" +
                                      $"{_fighters[1].TypeFighters} - Здоровье: {_fighters[1].CurrentHelth}");

                    _fighters[0].Attack(_fighters[1]);
                    _fighters[1].Attack(_fighters[0]);

                    numberOfRound++;

                    if (_fighters[0].CurrentHelth <= 0 || _fighters[1].CurrentHelth <= 0)
                    {
                        isFight = false;

                        Console.WriteLine(new string(FormatOutput.Delimeter, FormatOutput.DelimeterLenght));

                        if (_fighters[0].CurrentHelth <= 0 && _fighters[1].CurrentHelth <= 0)
                            Console.WriteLine("Ничья!! Противника пали одновременно.");
                        else if (_fighters[0].CurrentHelth <= 0)
                            Console.WriteLine($"Победил {_fighters[1].TypeFighters}.");
                        else
                            Console.WriteLine($"Победил {_fighters[0].TypeFighters}.");

                        Console.WriteLine(new string(FormatOutput.Delimeter, FormatOutput.DelimeterLenght));
                    }

                    Console.ReadKey(true);
                }
            }
            else
            {
                Console.WriteLine("Не набрано необходимое кол-во бойцов. Нужно 2.\n");
            }
        }

        private void ShowMenu()
        {
            Console.Clear();
            Console.WriteLine($"Выберите 2х бойцов для сражения на арене.\nПосле выбора выберите пункт \"Начать" +
                              $" сражение\".\n" + new string(FormatOutput.Delimeter, FormatOutput.DelimeterLenght));
            Console.WriteLine($"{CommantSelectMage} - Выбрать мага.\n{CommantSelectWarrior} - Выбрать война.\n" +
                              $"{CommantSelectBarbarian} - Выбрать варвара.\n{CommantSelectPaladin} - Выбрать " +
                              $"паладина.\n{CommantSelectArcher} - Выбрать лучника.\n{CommandStartFight} - " +
                              $"Начать сражение.\n{CommantExit} - Выйти из программы.\n" +
                              new string(FormatOutput.Delimeter, FormatOutput.DelimeterLenght));
            Console.WriteLine($"Выбрано бойцов: {_fighters.Count}\n" + new string(FormatOutput.Delimeter, FormatOutput.DelimeterLenght));
        }
    }

    class Fighter
    {
        protected int MaxHealth;
        protected int Damage;
        protected int Armor;

        public Fighter(TypeFighters type, int helthPoint, int damage, int armor)
        {
            TypeFighters = type;
            CurrentHelth = helthPoint;
            MaxHealth = helthPoint;
            Damage = damage;
            Armor = armor;
        }

        public TypeFighters TypeFighters { get; protected set; }
        public int CurrentHelth { get; protected set; }

        public virtual void SetDamage(int damage)
        {
            int calculateDamage = damage - Armor;
            calculateDamage = (calculateDamage < 0 ? 0 : calculateDamage);
            CurrentHelth -= calculateDamage;
            Console.WriteLine($"получает: {calculateDamage} ед. урона.");

            if (CurrentHelth < 0)
                CurrentHelth = 0;
        }

        public virtual void Attack(Fighter enemy)
        {
            Console.Write($"{TypeFighters} - Атакует.\t{enemy.TypeFighters} - ");
        }
    }

    class Mage : Fighter
    {
        private int _manaPoint;
        private int _shieldPoint;
        private Skill _skill;

        public Mage(TypeFighters type, int helthPoint, int damage, int armor, int manaPoint) : base(type, helthPoint, damage, armor)
        {
            _skill = new Skill("Energy Shield", 10, 30);
            _manaPoint = manaPoint;
        }

        public override void SetDamage(int damage)
        {
            int remainingDamage = damage - _shieldPoint;
            _shieldPoint -= damage;

            if (remainingDamage > 0)
                base.SetDamage(remainingDamage);
            else
                Console.WriteLine($"поглощает энерго-щитом урон, у щита остается {_shieldPoint} ед. прочности.");

            if (_shieldPoint <= 0)
            {
                _skill.OnActive = false;
                _shieldPoint = 0;
            }
        }

        public override void Attack(Fighter enemy)
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
                enemy.SetDamage(Damage);
            }
        }
    }

    class Warior : Fighter
    {
        private int _timeSkill;
        private Skill _skill;

        public Warior(TypeFighters type, int helthPoint, int damage, int armor) : base(type, helthPoint, damage, armor)
        {
            _skill = new Skill("Fortify", 3, 20);
        }

        public override void SetDamage(int damage)
        {
            base.SetDamage(damage);
        }

        public override void Attack(Fighter enemy)
        {
            if (_timeSkill > 0)
            {
                _timeSkill--;
            }
            else
            {
                _skill.OnActive = false;
                Armor -= _skill.Power;
            }

            if (_timeSkill <= 0 && _skill.OnActive == false)
            {
                _timeSkill = _skill.Cost;
                Armor += _skill.Power;
                _skill.OnActive = true;
                Console.WriteLine($"Боец использует {_skill.Name} и увеличивает защиту на {_skill.Power} едениц.");
            }
            else
            {
                base.Attack(enemy);
                enemy.SetDamage(Damage);
            }
        }
    }

    class Barbarian : Fighter
    {
        private int _rage;
        private int _percentageDamageAbsorbed;
        private Skill _skill;

        public Barbarian(TypeFighters type, int helthPoint, int damage, int armor, int percentageDamageAbsorbed) : base(type, helthPoint, damage, armor)
        {
            _skill = new Skill("Rage", 10, 25);
            _percentageDamageAbsorbed = percentageDamageAbsorbed;
        }

        public override void SetDamage(int damage)
        {
            int damageAbsorbed = (int)(_percentageDamageAbsorbed * ((float)damage / 100));
            _rage += damageAbsorbed;
            base.SetDamage(damage - damageAbsorbed);
        }

        public override void Attack(Fighter enemy)
        {
            if (_rage >= _skill.Cost)
            {
                Armor -= _skill.Cost;
                Damage += _skill.Power;
                _rage -= _skill.Cost;
                _skill.OnActive = true;
                Console.WriteLine($"{TypeFighters} использует {_skill.Name} и усиливает следующий удар на {_skill.Power} едениц.");
            }

            if (_skill.OnActive)
            {
                base.Attack(enemy);
                enemy.SetDamage(Damage);
                Armor += _skill.Cost;
                Damage -= _skill.Power;
                _skill.OnActive = false;
            }
            else
            {
                base.Attack(enemy);
                enemy.SetDamage(Damage);
            }
        }
    }

    class Paladin : Fighter
    {
        private int _faith;
        private Skill _skill;

        public Paladin(TypeFighters type, int helthPoint, int damage, int armor, int faith) : base(type, helthPoint, damage, armor)
        {
            _skill = new Skill("Heal", 10, 50);
            _faith = faith;
        }

        public override void SetDamage(int damage)
        {
            base.SetDamage(damage);
        }

        public override void Attack(Fighter enemy)
        {
            if (_faith >= _skill.Cost && CurrentHelth < (MaxHealth / 2))
            {
                CurrentHelth += _skill.Power;
                _faith -= _skill.Cost;
                Console.WriteLine($"Паладин использует {_skill.Name} и лечит себя на {_skill.Power} едениц.");
            }
            else
            {
                base.Attack(enemy);
                enemy.SetDamage(Damage);
            }
        }
    }

    class Archer : Fighter
    {
        private Random _random = new Random();
        private int _dodgeChance;
        private int _ammunition;

        public Archer(TypeFighters type, int helthPoint, int damage, int armor, int dodgeChance) : base(type, helthPoint, damage, armor)
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

        public override void Attack(Fighter enemy)
        {

            if (_ammunition <= 0)
            {
                _dodgeChance = 0;
                Damage /= 2;
            }

            base.Attack(enemy);
            enemy.SetDamage(Damage);
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

    internal enum TypeFighters
    {
        Mage = 1,
        Warior = 2,
        Barbarian = 3,
        Paladin = 4,
        Archer = 5,
    }
}