﻿using System;
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
        private const string CommandSelectFighters = "1";
        private const string CommandStartFight = "2";
        private const string CommandExit = "3";

        private int _maxFitersCount = 2;
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
                string menuNumber = Console.ReadLine();
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

                Console.WriteLine("Для возврашения в меню, нажмите любую клавишу...");
                Console.ReadKey(true);
            }
        }

        private void SelectFighters()
        {
            if (_selectedFighters.Count < _maxFitersCount)
            {
                ChooseFighter();
                ChooseFighter();
            }
            else if (_selectedFighters.Count > 1)
            {
                Console.Clear();
                Console.WriteLine($"Какого бойца вы хотите заменить?");

                for (int i = 0; i < _selectedFighters.Count; i++)
                    Console.Write($"{i + 1} - {_selectedFighters[i].TypeFighters}\n");

                if (int.TryParse(Console.ReadLine(), out int fighterNumber))
                {
                    fighterNumber--;

                    if (fighterNumber < _maxFitersCount && fighterNumber >= 0)
                    {
                        _selectedFighters.RemoveAt(fighterNumber);
                        ChooseFighter();
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

        private void ChooseFighter()
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
            if (_selectedFighters.Count == _maxFitersCount)
            {
                int numberOfRound = 1;
                bool isFight = true;

                Console.Clear();

                while (isFight)
                {
                    Console.WriteLine(new string(_delimeter, _delimeterLenght) + $"\nРаунд №{numberOfRound}.");

                    foreach (var fighter in _selectedFighters)
                        Console.Write($"{fighter.TypeFighters} - Здоровье: {fighter.CurrentHealth}\t");

                    Console.WriteLine();

                    _selectedFighters[0].Attack(_selectedFighters[1]);
                    _selectedFighters[1].Attack(_selectedFighters[0]);

                    numberOfRound++;

                    if (_selectedFighters[0].CurrentHealth <= 0 || _selectedFighters[1].CurrentHealth <= 0)
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

            if (_selectedFighters[0].CurrentHealth <= 0 && _selectedFighters[1].CurrentHealth <= 0)
                Console.WriteLine("Ничья!! Противники пали одновременно.");
            else if (_selectedFighters[0].CurrentHealth <= 0)
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
            CurrentHealth = helthPoint;
            MaxHealth = helthPoint;
            Damage = damage;
            Armor = armor;
        }

        public string TypeFighters { get; protected set; }
        public int CurrentHealth { get; protected set; }

        public virtual void TakeDamage(int damage)
        {
            int calculateDamage = damage - Armor;
            calculateDamage = (calculateDamage < 0 ? 0 : calculateDamage);
            CurrentHealth -= calculateDamage;
            Console.WriteLine($"получает: {calculateDamage} ед. урона.");

            if (CurrentHealth < 0)
                CurrentHealth = 0;
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

        public override void TakeDamage(int damage)
        {
            int remainingDamage = damage - _shieldPoint;
            _shieldPoint -= damage;

            if (remainingDamage > 0)
                base.TakeDamage(remainingDamage);
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
                enemy.TakeDamage(Damage);
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

        public override void TakeDamage(int damage)
        {
            base.TakeDamage(damage);
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
                enemy.TakeDamage(Damage);
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

        public override void TakeDamage(int damage)
        {
            int damageAbsorbed = (int)(_percentageDamageAbsorbed * ((float)damage / 100));
            _rage += damageAbsorbed;
            base.TakeDamage(damage - damageAbsorbed);
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
                enemy.TakeDamage(Damage);
                Armor += _skill.Cost;
                Damage -= _skill.Power;
                _skill.OnActive = false;
            }
            else
            {
                Console.Write($"{TypeFighters} - Атакует.\t{enemy.TypeFighters} - ");
                enemy.TakeDamage(Damage);
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

        public override void TakeDamage(int damage)
        {
            base.TakeDamage(damage);
        }

        public override void Attack(Fighter enemy)
        {
            float half = 0.5f;
            int halfHealth = (int)(MaxHealth * half);

            if (_faith >= _skill.Cost && CurrentHealth < halfHealth)
            {
                CurrentHealth += _skill.Power;
                _faith -= _skill.Cost;
                Console.WriteLine($"Паладин использует {_skill.Name} и лечит себя на {_skill.Power} едениц.");
            }
            else
            {
                Console.Write($"{TypeFighters} - Атакует.\t{enemy.TypeFighters} - ");
                enemy.TakeDamage(Damage);
            }
        }
    }

    class Archer : Fighter
    {
        private Random _random = new Random();

        private int _maxAmmunitionCount = 100;
        private int _minAmmunitionCount = 30;
        private int _currentAmmunition;
        private int _dodgeChance;

        public Archer(string type, int helthPoint, int damage, int armor, int dodgeChance) : base(type, helthPoint, damage, armor)
        {
            _dodgeChance = dodgeChance;
            _currentAmmunition = _random.Next(_minAmmunitionCount, _maxAmmunitionCount);
        }

        public override void TakeDamage(int damage)
        {
            if (_random.Next(100) > _dodgeChance)
                base.TakeDamage(damage);
            else
                Console.WriteLine("Лучник увернулся от удара");
        }

        public override void Attack(Fighter enemy)
        {
            if (_currentAmmunition <= 0)
            {
                _dodgeChance = 0;
                float half = 0.5f;
                Damage = (int)(Damage * half);
            }

            Console.Write($"{TypeFighters} - Атакует.\t{enemy.TypeFighters} - ");
            enemy.TakeDamage(Damage);
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