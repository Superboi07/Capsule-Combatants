using System;
using System.Threading.Tasks.Dataflow;

namespace Main
{
    // main logic class
    class Program
    {
        static readonly int BOARD_WIDTH = 6;
        static readonly int BOARD_HEIGHT = 7;

        static readonly int STARTING_POINTS = 35; // units cost points
        static readonly int STARTING_HEALTH = 100;
        static readonly int STARTING_ACTIONS = 3; // actions per turn

        static Board[] boards = [new(0, BOARD_WIDTH, BOARD_HEIGHT), new(1, BOARD_WIDTH, BOARD_HEIGHT)];
        static int[] playerPoints = [STARTING_POINTS, STARTING_POINTS];
        static int[] playerHealths = [STARTING_HEALTH, STARTING_HEALTH];
        static int[] playerActs = [STARTING_ACTIONS, STARTING_ACTIONS];

        static int gameState = 0; // 0 = main menu, 1 = player 1 unit select, 2 = player 2 unit select, 3 = game, 4 = end screen
        static int playerTurn = 0; // 0 = player 1, 1 = player 2

        // initzation method; for use until actual devcade IDE is used
        // run via terminal: [dotnet run --project Main]
        public static void Main(String[] args)
        {
            Initalize();
        }

        // does things which need to happen before loading content
        static void Initalize()
        {
            LoadContent();
        }

        // loads content
        static void LoadContent()
        {
            Update();
        }

        // primary update loop
        static void Update()
        {
            string? input = "";
            if (gameState == 3) // main game loop; out of order bc it's the most likely
            {
                boards[playerTurn].Print();
                input = Console.ReadLine();

                if (input == "clear") // debug only
                {
                    boards[playerTurn].Clear();
                    playerPoints[playerTurn] = STARTING_POINTS;
                }
                else if (input == "reset") // debug only
                {
                    boards[playerTurn].Clear();
                    playerPoints[playerTurn] = STARTING_POINTS;
                    playerPoints[playerTurn] = boards[playerTurn].Reinforce(playerPoints[playerTurn], true);
                }
                else if (input == "reinforce")
                {
                    playerPoints[playerTurn] = boards[playerTurn].Reinforce(playerPoints[playerTurn], false);
                }
                else if (input == "remove")
                {
                    int y;
                    int x;

                    if (int.TryParse(Console.ReadLine(), out y) && int.TryParse(Console.ReadLine(), out x))
                    {
                        int[] nums = boards[playerTurn].Remove(y, x);
                        playerPoints[playerTurn] += nums[0];
                        playerActs[playerTurn] += nums[1];
                    }
                    else
                    {
                        Console.WriteLine("Invalid input");
                        playerActs[playerTurn]++;
                    }
                }
                else if (input == "move")
                {
                    int from;
                    int to;

                    if (int.TryParse(Console.ReadLine(), out from) && int.TryParse(Console.ReadLine(), out to))
                    {
                        playerActs[playerTurn] += boards[playerTurn].Move(from, to);
                    }
                    else
                    {
                        playerActs[playerTurn]++;
                    }
                }
                else
                {
                    Console.WriteLine("Invalid input");
                    playerActs[playerTurn]++;
                }

                // decrease actions; if actions are zero: reset & pass turn
                playerActs[playerTurn]--;
                if (playerActs[playerTurn] == 0)
                {
                    playerActs[playerTurn] = STARTING_ACTIONS;

                    if (playerTurn == 0)
                    {
                        // playerTurn = 1;
                    }
                    else if (playerTurn == 1)
                    {
                        playerTurn = 0;
                    }
                    else
                    {
                        Console.WriteLine("paik; playerTurn not 0 nor 1");
                    }
                }
            }
            else if (gameState == 0) // main menu
            {
                gameState = 1;
            }
            else if (gameState == 1) // player 1 unit select
            {
                boards[0].SetDemo();
                playerPoints[0] = boards[0].Reinforce(playerPoints[0], true);
                gameState = 2;
            }
            else if (gameState == 2) // player 2 unit select
            {
                gameState = 3;
            }
            else if (gameState == 4) // end screen
            {
                return;
            }
            else
            {
                // PANIK
                Console.WriteLine("gameState: " + gameState + " is invalid");
                return;
            }

            if (input == "exit")
            {
                return;
            }
            Update();

        }
    }

    // handles the board & static elements
    class Board
    {
        static readonly Random rand = new();

        // ANSI codes, for use in debugging and pre-devcade demo
        public static readonly string ANSI_RESET = "\u001B[0m";
        public static readonly string ANSI_ITALICS = "\033[3m";
        public static readonly string[] ANSI_COLORS =
        [
            "\u001B[30m", "\u001B[31m", "\u001B[32m", "\u001B[34m"
        ]; // black, red, green, blue

        // all empty spaces should referece this object
        public static readonly Unit BLANK_UNIT = new("blank unit", "", '_', 0, 0, 0, 0, 0, 0, 0, 0);

        // first dimention is theme, second dimention is unit varity, third dimention is forme
        public static readonly Unit[][][] UNIT_LISTS =
        [[
            [
                new("demo defense 1", "black", 'x', 4, 0, 0, 0, 1, 0, 0, -1),
                new("demo defense 2", "black", 'X', 9, 0, 0, 0, 2, 0, 0, -2)
            ],
            [
                new("demo basic weak", "", 'u', 2, 5, 5, 1, 1, 0, 0, 0),
                new("demo basic strong", "", 'U', 3, 8, 6, 1, 1, 0, 0, 0)
            ],
            [
                new("demo special 1 top", "", '∧', 7, 12, 10, 2, 4, 1, 0, 1),
                new("demo special 1 bottom", "", '∨', 7, 12, 10, 2, 4, -1, 0, 1)
            ],
            [
                new("demo special 2 top-left", "", '◢', 15, 18, 10, 3, 7, 1, 1, 1),
                new("demo special 2 bottom-left", "", '◥', 15, 18, 10, 3, 7, -1, 1, 1),
                new("demo special 2 top-right", "", '◣', 15, 18, 10, 3, 7, 1, -1, 1),
                new("demo special 2 bottom-right", "", '◤', 15, 18, 10, 3, 7, -1, -1, 1)
            ]
        ]];

        public int chosenTheme = 0;
        public int[] chosenBasicFormes = new int[3];
        public int[] chosenSpecials = new int[2];
        public int[] special1Count = [0, 0]; // max, current
        public int[] special2Count = [0, 0]; // max, current
                                             // default maxes: 2x1 special, 2; 2x2 special, 1; 1x2 special, not implemented

        public Unit[,] board;
        readonly int player, width, height;

        public Board(int player, int width, int height)
        {
            this.player = player; // 0 = player 1, 1 = player 2
            this.width = width;
            this.height = height;
            board = new Unit[height, width];
            Clear();
        }

        public void Clear()
        {
            special1Count[1] = 0;
            special2Count[1] = 0;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    board[y, x] = BLANK_UNIT;
                }
            }
        }

        public void Print()
        {
            if (player == 0)
            {
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        Console.Write(ANSI_COLORS[board[y, x].colorCode] + board[y, x].symbol + ANSI_RESET);
                    }
                    Console.WriteLine();
                }
            }
            else if (player == 1) // player 2's board is visually flipped
            {
                for (int y = height - 1; y > -1; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        Console.Write(ANSI_COLORS[board[y, x].colorCode] + board[y, x].symbol + ANSI_RESET);
                    }
                    Console.WriteLine();
                }
            }
        }

        // sets choosenUnitList to demo values
        // blueprint for actual unit choice
        public void SetDemo()
        {
            chosenTheme = 0;
            chosenBasicFormes = [0, 1, 0];
            chosenSpecials = [2, 3];
            if (UNIT_LISTS[chosenTheme][chosenSpecials[0]].Length == 2)
            {
                special1Count[0] = 2;
            }
            else if (UNIT_LISTS[chosenTheme][chosenSpecials[0]].Length == 4)
            {
                special1Count[0] = 1;
            }
            else
            {
                Console.WriteLine("SetDemo error");
            }
            special1Count[1] = 0;
            if (UNIT_LISTS[chosenTheme][chosenSpecials[1]].Length == 2)
            {
                special2Count[0] = 2;
            }
            else if (UNIT_LISTS[chosenTheme][chosenSpecials[1]].Length == 4)
            {
                special2Count[0] = 1;
            }
            else
            {
                Console.WriteLine("SetDemo error");
            }
            special2Count[1] = 0;
        }

        // fills the board with random units
        public int Reinforce(int points, bool inital)
        {
            int[] odds = CalculateOdds(inital);
            while (points > 0)
            {
                int colorRand = rand.Next(3);
                string color;
                if (colorRand == 0)
                {
                    color = "red";
                }
                else if (colorRand == 1)
                {
                    color = "green";
                }
                else
                {
                    color = "blue";
                }

                int typeRand = rand.Next(100);
                Unit unit;
                if (typeRand < odds[0])
                {
                    unit = (Unit)UNIT_LISTS[chosenTheme][1][chosenBasicFormes[colorRand]].Clone();
                    unit.color = color;
                    unit.colorCode = colorRand + 1;
                    points -= FindBasicSpot(unit);
                }
                else if (typeRand < odds[0] + odds[1])
                {
                    unit = UNIT_LISTS[chosenTheme][chosenSpecials[0]][chosenBasicFormes[0]];
                    if (FindSpecialSpot(unit, 0, color, colorRand))
                    {
                        points -= unit.cost;
                        special1Count[1]++;
                        if (special1Count[0] == special1Count[1])
                        {
                            odds[0] += odds[1];
                            odds[1] = 0;
                        }
                    }
                }
                else
                {
                    unit = UNIT_LISTS[chosenTheme][chosenSpecials[1]][chosenBasicFormes[0]];
                    if (FindSpecialSpot(unit, 1, color, colorRand))
                    {
                        points -= unit.cost;
                        special2Count[1]++;
                        if (special2Count[0] == special2Count[1])
                        {
                            odds[0] += odds[2];
                            odds[2] = 0;
                        }
                    }
                }
            }
            return 0;
        }

        // calcs the odds for reinforcement units
        int[] CalculateOdds(bool inital)
        {
            int[] odds = new int[3];
            int temp = 2;
            if (!inital)
            {
                temp = 5;
            }

            if (special1Count[0] == special1Count[1])
            {
                odds[1] = 0;
            }
            else
            {
                odds[1] = (special1Count[0] + temp) * 5;
            }

            if (special2Count[0] == special2Count[1])
            {
                odds[2] = 0;
            }
            else
            {
                odds[2] = (special2Count[0] + temp) * 5;
            }

            odds[0] = 100 - (odds[1] + odds[2]);
            return odds;
        }

        // finds a spot for basic unit & places it; assumes reinforcing
        int FindBasicSpot(Unit unit)
        {
            List<int> col = [];
            List<int> row = [];

            for (int x = 0; x < width; x++)
            {
                int tempRow = height - 1;
                for (int y = 0; y < height && tempRow == height - 1; y++)
                {
                    if (board[y, x] != BLANK_UNIT)
                    {
                        tempRow = y - 1;
                    }
                }
                if (tempRow != -1)
                {
                    int[] pain = CheckDefenseMatch(unit.color, tempRow, x);
                    if (CheckAttackMatch(unit.color, tempRow, x) == 0 && pain[0] == 0 && pain[1] == 0)
                    {
                        col.Add(x);
                        row.Add(tempRow);
                    }
                }
            }

            if (col.Count == 0)
            {
                return 0;
            }

            int numRand = rand.Next(col.Count);
            board[row[numRand], col[numRand]] = unit;

            return 1;
        }

        // finds a spot for special unit & places it; 1x2 units not supported
        bool FindSpecialSpot(Unit unit, int specialNum, string color, int colorCode)
        {
            List<int> col = [];
            List<int> row = [];

            for (int x = 0; x < width - unit.horz; x++)
            {
                int tempRow = height - 2;
                for (int y = 0; y < height && tempRow == height - 2; y++)
                {
                    if (board[y, x] != BLANK_UNIT || board[y, x + unit.horz] != BLANK_UNIT)
                    {
                        tempRow = y - 2;
                    }
                }
                if (tempRow >= 0)
                {
                    col.Add(x);
                    row.Add(tempRow);
                }
            }

            if (col.Count == 0)
            {
                return false;
            }

            int numRand = rand.Next(col.Count);
            for (int y = 0; y <= 1; y++)
            {
                for (int x = 0; x <= unit.horz; x++)
                {
                    Console.WriteLine("s " + specialNum);
                    Console.WriteLine("r " + row[numRand] + " " + y);
                    Console.WriteLine("c " + col[numRand] + " " + x);
                    board[row[numRand] + y, col[numRand] + x] = (Unit)UNIT_LISTS[chosenTheme][chosenSpecials[specialNum]][y + (x * 2)].Clone();
                    board[row[numRand] + y, col[numRand] + x].color = color;
                    board[row[numRand] + y, col[numRand] + x].colorCode = colorCode + 1;
                }
            }

            return true;
        }

        // checks if there's 3 in a row vertical
        int CheckAttackMatch(string color, int y, int x)
        {
            if (y + 2 < height && board[y + 1, x].color == color && board[y + 1, x].special == 0 && !board[y + 1, x].attacking)
            { // if theres enough room, the unit direcly below is a basic, the color matches, and isn't attacking
                if (board[y + 2, x].color == color && !board[y + 1, x].attacking)
                { // checks the color and attack of the unit 2 below
                    if (board[y + 2, x].special == 0)
                    { // if basic
                        return 1;
                    }
                    else if (board[y, x + board[y, x].horz].special == 0 && board[y, x + board[y, x].horz].color == color && board[y + 1, x + board[y, x].horz].special == 0 && board[y + 1, x + board[y, x].horz].color == color)
                    { // if the two units to the side are basic and match color; assumes specials less than 0 don't have color other than black, and that the units to the side can't attack
                        return 2;
                    }
                }
            }
            return 0;
        }

        // checks if there's 3+ in a row horizontal
        int[] CheckDefenseMatch(string color, int y, int x)
        {
            int left = 0;
            int right = 0;

            if (x > 0 && board[y, x - 1].special == 0 && board[y, x - 1].color == color && !board[y, x - 1].attacking)
            {
                left++;
                if (x > 1 && board[y, x - 2].special == 0 && board[y, x - 2].color == color && !board[y, x - 2].attacking)
                {
                    left++;
                }
            }

            if (x < width - 1 && board[y, x + 1].special == 0 && board[y, x + 1].color == color && !board[y, x + 1].attacking)
            {
                right++;
                if (x < width - 2 && board[y, x + 2].special == 0 && board[y, x + 2].color == color && !board[y, x + 2].attacking)
                {
                    right++;
                }
            }

            return [left, right];
        }

        // removes unit at spot
        public int[] Remove(int y, int x)
        {
            int[] nums = [board[y, x].cost, 0];
            int horz = board[y, x].horz;
            int vert = board[y, x].vert;

            if (vert != 0)
            {
                board[y + vert, x] = BLANK_UNIT;
                if (vert == 1)
                {
                    vert = 0;
                }

                if (horz != 0)
                {
                    board[y, x + horz] = BLANK_UNIT;
                    board[y + board[y, x].vert, x + horz] = BLANK_UNIT;
                    Down(y + vert - 1, x + horz);
                }
            }

            board[y, x] = BLANK_UNIT;
            Down(y + vert - 1, x);

            if (horz == 0)
            {
                nums[1] = CheckRow(x);
            }
            else
            {
                // pain
            }

            return nums;
        }

        // moves top unit from col to col
        public int Move(int from, int to)
        {
            int acts = 0;
            return acts;
        }

        // shifts units down; assumes position has blank below
        void Down(int y, int x)
        {
            if (y < 0 || board[y, x] == BLANK_UNIT) return;

            int pos = height - 1;
            for (int i = y + 2; i < height && pos == height - 1; i++)
            {
                if (board[y, x] != BLANK_UNIT)
                {
                    pos = i - 1;
                }
            }

            int vert = board[y, x].vert;
            int horz = board[y, x].horz;
            if (horz != 0)
            {
                int pos2 = height - 1;
                for (int i = y; i < height && pos2 == height - 1; i++)
                {
                    if (board[y, x + horz] != BLANK_UNIT)
                    {
                        pos2 = i - 1;
                    }
                }
                if (pos2 == y) return;
                pos = Math.Min(pos, pos2);
                board[pos, x] = board[y, x];
                board[pos + 1, x] = board[y + 1, x];
                board[pos, x + horz] = board[y, x + horz];
                board[pos + 1, x + horz] = board[y + 1, x + horz];
                board[y, x] = BLANK_UNIT;
                board[y + 1, x] = BLANK_UNIT;
                board[y, x + horz] = BLANK_UNIT;
                board[y + 1, x + horz] = BLANK_UNIT;
                Down(y - 2, x + horz);
            }
            else
            {
                board[pos, x] = board[y, x];
                if (vert == 1)
                {
                    board[pos + 1, x] = board[y + 1, x];
                    board[y + 1, x] = BLANK_UNIT;
                }
                board[y, x] = BLANK_UNIT;
            }
            Down(y - (1 + vert), x);
        }

        void Up(int y, int x, int dist)
        {
        }

        int CheckRow(int x)
        {
            return 0;
        }
    }

    // handles individual units
    class Unit
    {
        public string name, color;
        public int colorCode; // debug & pre-devcade demo use only
        public char symbol; // debug & pre-devcade demo use only
        /* public [] sprite; */
        public bool attacking;
        public int health, attack, growth, speed, cost;
        public int vert, horz; // for use in speical unit sizes
        public int special;
        // -2 = defense teir 2
        // -1 = defense teir 1
        // 0 = basic unit & blank
        // 1+ = special unit
        // 1 = special unit without bonus effect 
        // 2 = double health
        // 3 = blast
        public Unit(string name, string color, char symbol, int health, int attack, int growth, int speed, int cost, int vert, int horz, int special)
        {
            this.name = name;
            this.color = color;
            colorCode = 0;
            this.symbol = symbol;
            this.health = health;
            this.attack = attack;
            this.growth = growth;
            this.speed = speed;
            this.cost = cost;
            this.vert = vert;
            this.horz = horz;
            this.special = special;
            attacking = false;
        }

        public Unit Clone()
        {
            return (Unit)MemberwiseClone();
        }
    }
}