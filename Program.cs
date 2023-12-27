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

    class Arena
    {
        private const int CommandSelectFighters = 1;
        private const int CommandStartFight = 2;
        private const int CommandExit = 3;

        private int _delimeterLenght = 50;
        private char _delimeter = '=';

        private List<FighterCreator> _listFighters;
        private List<Fighter> _selectedFighters = new List<Fighter>();

        public Arena()
        {
            _listFighters = new List<FighterCreator> { new FighterCreatorMage(),
                                                       new FighterCreatorWarior(),
                                                       new FighterCreatorBarbarian(),
                                                       new FighterCreatorPaladin(),
                                                       new FighterCreatorArcher()};
        }

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
                        case CommandSelectFighters:
                            SelectFighters();
                            break;

                        case CommandStartFight:
                            Fight();
                            break;

                        case CommandExit:
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
            }
        }

        private void SelectFighters()
        {
            if (_selectedFighters.Count < 2)
            {
                AddFiter();
                AddFiter();
            }
            else if (_selectedFighters.Count > 1)
            {
                Console.Clear();
                Console.WriteLine($"Какого бойца вы хотите заменить?\n1 - {_selectedFighters[0].TypeFighters}\n2 - {_selectedFighters[1].TypeFighters}");

                if (int.TryParse(Console.ReadLine(), out int fighterNumber))
                {
                    fighterNumber--;

                    if (fighterNumber < 2 && fighterNumber >= 0)
                    {
                        _selectedFighters.RemoveAt(fighterNumber);
                        AddFiter();
                    }
                    else
                    {
                        ShowError();
                    }
                }
                else
                {
                    ShowError();
                }
            }
        }

        private void AddFiter()
        {
            Console.Clear();

            for (int i = 0; i < _listFighters.Count; i++)
                Console.WriteLine($"{i + 1} - {_listFighters[i].GetTypeName()}");

            Console.Write("Введите номер бойца: ");

            if (int.TryParse(Console.ReadLine(), out int fighterNumber))
            {
                fighterNumber--;

                if (fighterNumber < _listFighters.Count || fighterNumber >= 0)
                    _selectedFighters.Add(_listFighters[fighterNumber].Create());
                else
                    ShowError();
            }
            else
            {
                ShowError();
            }
        }

        private void Fight()
        {
            if (_selectedFighters.Count == 2)
            {
                int numberOfRound = 1;
                bool isFight = true;

                Console.Clear();

                while (isFight)
                {
                    Console.WriteLine(new string(_delimeter, _delimeterLenght) + $"\nРаунд №{numberOfRound}.");
                    Console.WriteLine($"{_selectedFighters[0].TypeFighters} - Здоровье: {_selectedFighters[0].CurrentHelth}\t" +
                                      $"{_selectedFighters[1].TypeFighters} - Здоровье: {_selectedFighters[1].CurrentHelth}");

                    _selectedFighters[0].Attack(_selectedFighters[1]);
                    _selectedFighters[1].Attack(_selectedFighters[0]);

                    numberOfRound++;

                    if (_selectedFighters[0].CurrentHelth <= 0 || _selectedFighters[1].CurrentHelth <= 0)
                        isFight = false;

                    Console.ReadKey(true);
                }

                ShowWinner();
            }
            else
            {
                Console.WriteLine("Не набрано необходимое кол-во бойцов. Нужно 2.\n");
            }
        }

        private void ShowWinner()
        {
            Console.WriteLine(new string(_delimeter, _delimeterLenght));

            if (_selectedFighters[0].CurrentHelth <= 0 && _selectedFighters[1].CurrentHelth <= 0)
                Console.WriteLine("Ничья!! Противники пали одновременно.");
            else if (_selectedFighters[0].CurrentHelth <= 0)
                Console.WriteLine($"Победил {_selectedFighters[1].TypeFighters}.");
            else
                Console.WriteLine($"Победил {_selectedFighters[0].TypeFighters}.");

            Console.WriteLine(new string(_delimeter, _delimeterLenght));
        }

        private void ShowMenu()
        {
            Console.Clear();
            Console.WriteLine($"Выберите 2х бойцов для сражения на арене.\n" + new string(_delimeter, _delimeterLenght));
            Console.WriteLine($"{CommandSelectFighters} - Выбрать бойцов.\n{CommandStartFight} - " +
                              $"Начать сражение.\n{CommandExit} - Выйти из программы.\n" +
                              new string(_delimeter, _delimeterLenght));
            Console.WriteLine($"Выбрано бойцов: {_selectedFighters.Count}\n" + new string(_delimeter, _delimeterLenght));
        }

        private void ShowError()
        {
            Console.WriteLine("\nВы ввели некорректное значение.");
        }
    }

    abstract class Fighter
    {
        protected int MaxHealth;
        protected int Damage;
        protected int Armor;

        public Fighter(string type, int helthPoint, int damage, int armor)
        {
            TypeFighters = type;
            CurrentHelth = helthPoint;
            MaxHealth = helthPoint;
            Damage = damage;
            Armor = armor;
        }

        public string TypeFighters { get; protected set; }
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

        public abstract void Attack(Fighter enemy);
    }

    class Mage : Fighter
    {
        private int _manaPoint;
        private int _shieldPoint;
        private Skill _skill;

        public Mage(string type, int helthPoint, int damage, int armor, int manaPoint) : base(type, helthPoint, damage, armor)
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
                Console.Write($"{TypeFighters} - Атакует.\t{enemy.TypeFighters} - ");
                enemy.SetDamage(Damage);
            }
        }
    }

    class Warior : Fighter
    {
        private int _timeSkill;
        private Skill _skill;

        public Warior(string type, int helthPoint, int damage, int armor) : base(type, helthPoint, damage, armor)
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
                Console.Write($"{TypeFighters} - Атакует.\t{enemy.TypeFighters} - ");
                enemy.SetDamage(Damage);
            }
        }
    }

    class Barbarian : Fighter
    {
        private int _rage;
        private int _percentageDamageAbsorbed;
        private Skill _skill;

        public Barbarian(string type, int helthPoint, int damage, int armor, int percentageDamageAbsorbed) : base(type, helthPoint, damage, armor)
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
                Console.Write($"{TypeFighters} - Атакует.\t{enemy.TypeFighters} - ");
                enemy.SetDamage(Damage);
                Armor += _skill.Cost;
                Damage -= _skill.Power;
                _skill.OnActive = false;
            }
            else
            {
                Console.Write($"{TypeFighters} - Атакует.\t{enemy.TypeFighters} - ");
                enemy.SetDamage(Damage);
            }
        }
    }

    class Paladin : Fighter
    {
        private int _faith;
        private Skill _skill;

        public Paladin(string type, int helthPoint, int damage, int armor, int faith) : base(type, helthPoint, damage, armor)
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
                Console.Write($"{TypeFighters} - Атакует.\t{enemy.TypeFighters} - ");
                enemy.SetDamage(Damage);
            }
        }
    }

    class Archer : Fighter
    {
        private Random _random = new Random();
        private int _dodgeChance;
        private int _ammunition;

        public Archer(string type, int helthPoint, int damage, int armor, int dodgeChance) : base(type, helthPoint, damage, armor)
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

            Console.Write($"{TypeFighters} - Атакует.\t{enemy.TypeFighters} - ");
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

    abstract class FighterCreator
    {
        public abstract Fighter Create();

        public abstract string GetTypeName();
    }

    class FighterCreatorMage : FighterCreator
    {
        public override Fighter Create() => new Mage(GetTypeName(), 200, 35, 0, 40);

        public override string GetTypeName() => "Mage";
    }

    class FighterCreatorWarior : FighterCreator
    {
        public override Fighter Create() => new Warior(GetTypeName(), 350, 15, 15);

        public override string GetTypeName() => "Warior";
    }

    class FighterCreatorBarbarian : FighterCreator
    {
        public override Fighter Create() => new Barbarian(GetTypeName(), 400, 25, 7, 30);

        public override string GetTypeName() => "Barbarian";
    }

    class FighterCreatorPaladin : FighterCreator
    {
        public override Fighter Create() => new Paladin(GetTypeName(), 300, 20, 10, 35);

        public override string GetTypeName() => "Paladin";
    }

    class FighterCreatorArcher : FighterCreator
    {
        public override Fighter Create() => new Archer(GetTypeName(), 275, 27, 5, 25);

        public override string GetTypeName() => "Archer";
    }
}