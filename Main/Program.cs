using System;
using System.Globalization;
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
        static readonly int STARTING_ACTIONS = 4; // actions per turn
        static readonly int PLAYER_1_BALANCE = 2; // how many less actions player 2 starts with

        static Board[] boards = [new(0, BOARD_WIDTH, BOARD_HEIGHT), new(1, BOARD_WIDTH, BOARD_HEIGHT)];
        public static int[] playerPoints = [STARTING_POINTS, STARTING_POINTS];
        static int[] playerHealths = [STARTING_HEALTH, STARTING_HEALTH];
        static int[] playerActs = [STARTING_ACTIONS - PLAYER_1_BALANCE, STARTING_ACTIONS];

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
                boards[0].Print();
                Console.WriteLine("~~~~~~");
                boards[1].Print();
                Console.WriteLine("\nPlayer: " + (playerTurn + 1));
                Console.WriteLine("Health: " + playerHealths[playerTurn]);
                Console.WriteLine("Points: " + playerPoints[playerTurn]);
                Console.WriteLine("Actions: " + playerActs[playerTurn] + "\n");
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
                    playerActs[playerTurn] += 10;
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
                        if (nums[1] != -1)
                        {
                            playerPoints[playerTurn] += nums[0];
                            playerActs[playerTurn] += nums[1];
                        }
                        else
                        {
                            Console.WriteLine("Invalid input");
                            playerActs[playerTurn]++;
                        }
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
                        int temp = boards[playerTurn].Move(from, to);
                        if (temp != -1)
                        {
                            if (temp > 0) temp--;
                            playerActs[playerTurn] += temp;
                        }
                        else
                        {
                            Console.WriteLine("Invalid input");
                            playerActs[playerTurn]++;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid input");
                        playerActs[playerTurn]++;
                    }
                }
                else if (input == "pass")
                {
                    playerActs[playerTurn] = 1;
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
                        int[] temp = boards[0].TurnEnd(boards[1].TurnStart());
                        playerHealths[0] -= temp[0];
                        if (playerHealths[0] <= 0)
                        {
                            Console.WriteLine("Player 1 wins!");
                            gameState = 4;
                        }
                        else
                        {
                            playerPoints[0] += temp[1];
                            playerTurn = 1;
                        }

                    }
                    else if (playerTurn == 1)
                    {
                        int[] temp = boards[1].TurnEnd(boards[0].TurnStart());
                        playerHealths[1] -= temp[0];
                        if (playerHealths[1] <= 0)
                        {
                            Console.WriteLine("Player 2 wins!");
                            gameState = 4;
                        }
                        else
                        {
                            playerPoints[1] += temp[1];
                            playerTurn = 0;
                        }
                    }
                    else
                    {
                        throw new SystemException("panik; playerTurn not 0 nor 1");
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
                boards[1].SetDemo();
                playerPoints[1] = boards[1].Reinforce(playerPoints[1], true);
                gameState = 3;
            }
            else if (gameState == 4) // end screen
            {
                return;
            }
            else
            {
                throw new SystemException("gameState: " + gameState + " is invalid");
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
        public static readonly string ANSI_ITALICS = "\u001b[3m";
        public static readonly string[] ANSI_COLORS =
        [
            "\u001B[30m", "\u001B[31m", "\u001B[32m", "\u001B[34m"
        ]; // black, red, green, blue

        // all empty spaces should referece this object
        public static readonly Unit BLANK_UNIT = new("blank unit", "", '_', 0, 0, 0, 0, 0, 0, 0, 0);

        // first dimention is theme, second dimention is unit varity, third dimention is forme
        public static readonly Unit[][][] UNIT_LISTS =
        [[ // string name, string color, char symbol, int health, int attack, int growth, int speed, int cost, int vert, int horz, int special
            [
                new("demo defense 1", "black", 'x', 4, 0, 0, 0, 1, 0, 0, -1),
                new("demo defense 2", "black", 'X', 9, 0, 0, 0, 2, 0, 0, -2)
            ],
            [
                new("demo basic weak", "", 'u', 2, 4, 6, 1, 1, 0, 0, 0),
                new("demo basic strong", "", 'U', 3, 6, 6, 1, 2, 0, 0, 0)
            ],
            [
                new("demo special 1 top", "", 'Ʌ', 7, 12, 10, 2, 4, 1, 0, 1),
                new("demo special 1 bottom", "", 'V', 7, 12, 10, 2, 4, -1, 0, 1)
            ],
            [
                new("demo special 2 top-left", "", '/', 15, 18, 10, 3, 7, 1, 1, 1),
                new("demo special 2 bottom-left", "", '\\', 15, 18, 10, 3, 7, -1, 1, 1),
                new("demo special 2 top-right", "", '\\', 15, 18, 10, 3, 7, 1, -1, 1),
                new("demo special 2 bottom-right", "", '/', 15, 18, 10, 3, 7, -1, -1, 1)
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
                        if (board[y, x].attacking) Console.Write(ANSI_ITALICS);
                        Console.Write(ANSI_COLORS[board[y, x].colorCode] + board[y, x].symbol + ANSI_RESET);
                    }
                    Console.WriteLine();
                }
            }
            else if (player == 1) // player 2's board is visually flipped
            {
                for (int y = height - 1; y > -1; y--)
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
            chosenSpecials = [2, 3]; // 2 1x2 allowed, 2 2x2 not allowed; multible systems assume only 1 2 wide unit
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
                throw new SystemException("SetDemo error");
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
                throw new SystemException("SetDemo error");
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
                    if (CheckAttackMatch(unit.color, tempRow, x) == 0 && CheckDefenseMatchLeft(unit.color, tempRow, x) + CheckDefenseMatchRight(unit.color, tempRow, x) < 2)
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
                    board[row[numRand] + y, col[numRand] + x] = UNIT_LISTS[chosenTheme][chosenSpecials[specialNum]][y + (x * 2)].Clone();
                    board[row[numRand] + y, col[numRand] + x].color = color;
                    board[row[numRand] + y, col[numRand] + x].colorCode = colorCode + 1;
                    board[row[numRand] + y, col[numRand] + x].matchAttackID = specialNum + 2;
                    if (player == 1)
                    {
                        board[row[numRand] + y, col[numRand] + x].symbol = UNIT_LISTS[chosenTheme][chosenSpecials[specialNum]][1 - y + (x * 2)].symbol;
                    }
                }
            }

            return true;
        }

        // removes unit at spot
        public int[] Remove(int y, int x)
        {
            if (y >= height || y < 0 || x >= width || x < 0 || board[y, x] == BLANK_UNIT || board[y, x].attacking)
            { // can't remove blank unit or attacking units
                return [-1, -1];
            }

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

            nums[1] = CheckBoard();

            return nums;
        }

        // moves top unit from col to col
        public int Move(int from, int to)
        {
            if (from >= width || from < 0 || to >= width || to < 0)
            {
                return -1;
            }
            int fromY = -1;
            int toY = height - 1;
            for (int i = 0; i < height && fromY == -1; i++)
            { // finds the top from unit
                if (board[i, from] != BLANK_UNIT)
                {
                    if (board[i, from].attacking || board[i, from].special < 0)
                    {
                        return -1;
                    }
                    fromY = i;
                }
            }
            for (int i = 0; i < height && toY == height - 1; i++)
            { // finds the to space
                if (board[i, to] != BLANK_UNIT)
                {
                    toY = i - 1;
                }
            }
            if (fromY == -1 || toY == -1)
            {
                return -1;
            }

            if (board[fromY, from].vert == 0)
            {
                board[toY, to] = board[fromY, from];
                board[fromY, from] = BLANK_UNIT;
                return CheckBoard(); // pain
            }
            else
            {
                if (toY == 0)
                {
                    return -1;
                }

                if (board[fromY, from].horz == 0)
                {
                    board[toY - 1, to] = board[fromY, from];
                    board[toY, to] = board[fromY + 1, from];
                    board[fromY, from] = BLANK_UNIT;
                    board[fromY + 1, from] = BLANK_UNIT;
                }
                else
                {
                    if (to == 0) return -1;
                    if (fromY != 0 && board[fromY - 1, from + board[fromY, from].horz] != BLANK_UNIT)
                    {
                        return -1;
                    }

                    Unit temp1 = board[fromY, from]; // insures no overlap issues
                    Unit temp2 = board[fromY, from - 1];
                    Unit temp3 = board[fromY + 1, from];
                    Unit temp4 = board[fromY + 1, from - 1];

                    board[fromY, from] = BLANK_UNIT;
                    board[fromY, from - 1] = BLANK_UNIT;
                    board[fromY + 1, from] = BLANK_UNIT;
                    board[fromY + 1, from - 1] = BLANK_UNIT;

                    int toY2 = height - 1;
                    for (int i = 0; i < height && toY2 == height - 1; i++)
                    { // finds the to2 space
                        if (board[i, to - 1] != BLANK_UNIT)
                        {
                            toY2 = i - 1;
                        }
                    }
                    if (toY2 < 1)
                    {
                        board[fromY, from] = temp1;
                        board[fromY, from - 1] = temp2;
                        board[fromY + 1, from] = temp3;
                        board[fromY + 1, from - 1] = temp4;
                        return -1;
                    }

                    toY = Math.Min(toY, toY2);
                    if (board[fromY, from].horz == 1) from--; // from should be on the right side

                    board[toY - 1, to] = temp1;
                    board[toY - 1, to - 1] = temp2;
                    board[toY, to] = temp3;
                    board[toY, to - 1] = temp4;
                }
                return 0;
            }

        }

        // shifts units down; assumes position has blank below
        void Down(int y, int x)
        {
            if (y < 0 || board[y, x] == BLANK_UNIT) return;

            int pos = height - 1;
            for (int i = y + 2; i < height && pos == height - 1; i++)
            {
                if (board[i, x] != BLANK_UNIT)
                {
                    pos = i - 1;
                }
            }

            int vert = board[y, x].vert;
            int horz = board[y, x].horz;
            if (horz != 0)
            {
                int pos2 = height - 1;
                for (int i = y + 1; i < height && pos2 == height - 1; i++)
                {
                    if (board[i, x + horz] != BLANK_UNIT)
                    {
                        pos2 = i - 1;
                    }
                }

                if (pos2 == y) return;
                pos = Math.Min(pos, pos2);

                board[pos, x] = board[y, x];
                board[pos, x + horz] = board[y, x + horz];
                board[pos - 1, x] = board[y - 1, x];
                board[pos - 1, x + horz] = board[y - 1, x + horz];

                if (pos != y + 1)
                {
                    board[y, x] = BLANK_UNIT;
                    board[y, x + horz] = BLANK_UNIT;
                }
                board[y - 1, x] = BLANK_UNIT;
                board[y - 1, x + horz] = BLANK_UNIT;

                Down(y - 2, x + horz);
            }
            else
            {
                board[pos, x] = board[y, x];
                if (vert == -1)
                {
                    board[pos - 1, x] = board[y - 1, x];
                    board[y - 1, x] = BLANK_UNIT;
                    if (y + 1 != pos)
                    {
                        board[y, x] = BLANK_UNIT;
                    }
                }
                else
                {
                    board[y, x] = BLANK_UNIT;
                }
            }
            Down(y + vert - 1, x);
        }

        void Up(int y, int x, int dist)
        {
            if (y - dist < 0)
            {
                if (board[y, x].attacking)
                {
                    throw new Exception("an attacking unit shouldn't be able to exit from top.");
                }
                Program.playerPoints[player] += board[y, x].cost;
                if (board[y, x].vert != 0)
                {
                    board[y + 1, x] = BLANK_UNIT;
                    if (board[y, x].horz != 0)
                    {
                        board[y, x + board[y, x].horz] = BLANK_UNIT;
                        board[y + 1, x + board[y, x].horz] = BLANK_UNIT;
                    }
                }
                board[y, x] = BLANK_UNIT;
            }
            else if (board[y, x] == BLANK_UNIT)
            {
                return;
            }
            else
            {
                if (board[y, x].horz != 0)
                {
                    Up(y - 2, x, 1);
                    Up(y - 2, x + board[y, x].horz, 1);
                    board[y - 2, x + board[y, x].horz] = board[y - 1, x + board[y, x].horz];
                    board[y - 1, x + board[y, x].horz] = board[y, x + board[y, x].horz];
                    board[y - 2, x] = board[y - 1, x];
                    board[y - 1, x] = board[y, x];
                    board[y, x + board[y, x].horz] = BLANK_UNIT;
                }
                else
                {
                    Up(y - 1, x, 1);
                    board[y - 1, x] = board[y, x];
                }
            }

            if (dist != 1) Up(y - 1, x, dist - 1);
        }

        // will initizate match; I didn't want to check the entire board every time, but I gave up
        int CheckBoard()
        {
            int matches = 0;

            for (int y = 0; y < height; y++)
            { // check each spot
                for (int x = 0; x < width; x++)
                {
                    if (board[y, x] != BLANK_UNIT && board[y, x].special == 0)
                    {
                        matches += CheckSpot(y, x);
                    }
                }
            }

            if (matches != 0)
            {
                // def to add
                int[] colDefStates = [0, 0, 0, 0, 0, 0];
                // colorCode, matchAttackID
                // 0 = none, 1 = basic, 2 = special 1, 3 = special 2, 4 = two at once (not implemented)
                int[][] colAtkStates = [[0, 0], [0, 0], [0, 0], [0, 0], [0, 0], [0, 0]];

                for (int y = 0; y < height; y++)
                { // remove all matched units and apply correct states
                    for (int x = 0; x < width; x++)
                    {
                        if (board[y, x].matchDefend)
                        {
                            colDefStates[x]++;
                            if (board[y, x].matchAttack == 1)
                            {
                                if (colAtkStates[x][1] != 0)
                                {
                                    throw new SystemException("Multible attacks initating simultaneously in the same column not supported. If you are reading this during an actual game, player: " + player + " wins for acheving the impossible.");
                                }
                                else
                                {
                                    colAtkStates[x][0] = board[y, x].colorCode;
                                    colAtkStates[x][1] = 1;
                                    board[y + 1, x] = BLANK_UNIT;
                                    board[y + 2, x] = BLANK_UNIT;
                                }
                            }
                            else if (board[y, x].matchAttack == 2)
                            {
                                if (colAtkStates[x][1] != 0)
                                {
                                    throw new SystemException("Multible attacks initating simultaneously in the same column not supported. If you are reading this during an actual game, player: " + player + " wins for acheving the impossible.");
                                }
                                else
                                {
                                    colAtkStates[x][0] = board[y, x].colorCode;
                                    colAtkStates[x][1] = board[y + 2, x].matchAttackID;
                                    if (board[y + 2, x].horz != 0)
                                    {
                                        board[y, x + 1] = BLANK_UNIT;
                                        board[y + 1, x + 1] = BLANK_UNIT;
                                        board[y + 2, x + 1] = BLANK_UNIT;
                                        board[y + 3, x + 1] = BLANK_UNIT;
                                    }
                                    board[y + 1, x] = BLANK_UNIT;
                                    board[y + 2, x] = BLANK_UNIT;
                                    board[y + 3, x] = BLANK_UNIT;
                                }
                            }
                            board[y, x] = BLANK_UNIT;
                        }
                        else
                        {
                            if (board[y, x].matchAttack == 1)
                            {
                                if (colAtkStates[x][1] != 0)
                                {
                                    throw new SystemException("Multible attacks initating simultaneously in the same column not supported. If you are reading this during an actual game, player: " + player + " wins for acheving the impossible.");
                                }
                                else
                                {
                                    colAtkStates[x][0] = board[y, x].colorCode;
                                    colAtkStates[x][1] = 1;
                                    board[y, x] = BLANK_UNIT;
                                    board[y + 1, x] = BLANK_UNIT;
                                    board[y + 2, x] = BLANK_UNIT;
                                }
                            }
                            else if (board[y, x].matchAttack == 2)
                            {
                                if (colAtkStates[x][1] != 0)
                                {
                                    throw new SystemException("Multible attacks initating simultaneously in the same column not supported. If you are reading this during an actual game, player: " + player + " wins for acheving the impossible.");
                                }
                                else
                                {
                                    colAtkStates[x][0] = board[y, x].colorCode;
                                    colAtkStates[x][1] = board[y + 2, x].matchAttackID;
                                    if (board[y + 2, x].horz != 0)
                                    {
                                        board[y, x + 1] = BLANK_UNIT;
                                        board[y + 1, x + 1] = BLANK_UNIT;
                                        board[y + 2, x + 1] = BLANK_UNIT;
                                        board[y + 3, x + 1] = BLANK_UNIT;
                                    }
                                    board[y, x] = BLANK_UNIT;
                                    board[y + 1, x] = BLANK_UNIT;
                                    board[y + 2, x] = BLANK_UNIT;
                                    board[y + 3, x] = BLANK_UNIT;
                                }
                            }
                        }
                    }
                }

                for (int x = 0; x < width; x++)
                {
                    if (colDefStates[x] != 0 || colAtkStates[x][1] != 0)
                    {
                        for (int y = height - 1; y > 0; y--)
                        {
                            if (board[y, x] == BLANK_UNIT)
                            {
                                Down(y - 1, x);
                            }
                        }
                    }
                }

                for (int x = 0; x < width; x++)
                {
                    AddDefence(x, colDefStates[x]);
                }
                for (int x = 0; x < width; x++)
                {
                    AddAttack(x, colAtkStates[x]);
                }

                // do it all again
                matches += CheckBoard();
            }
            return matches;
        }

        // assumes unit at spot
        int CheckSpot(int y, int x)
        {
            int matches = 0;
            string color = board[y, x].color;
            int defPos = 0;
            int attackState = 0;
            if (!board[y, x].matchDefend)
            {
                defPos = CheckDefenseMatchRight(color, y, x);
            }
            if (board[y, x].matchAttack == 0)
            {
                attackState = CheckAttackMatch(color, y, x);
            }

            if (defPos > 1)
            {
                matches++;
                for (int i = 0; i <= defPos; i++)
                {
                    board[y, x + i].matchDefend = true;
                }
            }

            if (attackState == 1)
            {
                matches++;
                board[y, x].matchAttack = 1;
                board[y + 1, x].matchAttack = 1;
                board[y + 2, x].matchAttack = 1;
            }
            else if (attackState == 2)
            {
                matches++;
                board[y, x].matchAttack = 2;
                board[y + 1, x].matchAttack = 2;
                board[y + 2, x].matchAttack = 2;
                board[y + 2, x].matchAttack = 2;
                board[y, x + board[y + 2, x].horz].matchAttack = 2;
                board[y + 1, x + board[y + 2, x].horz].matchAttack = 2;
                board[y + 2, x + board[y + 2, x].horz].matchAttack = 2;
                board[y + 2, x + board[y + 2, x].horz].matchAttack = 2;
            }

            return matches;
        }

        // checks if there's 3 in a row vertical; shouldn't assume unit at spot
        int CheckAttackMatch(string color, int y, int x)
        {
            if (y + 2 < height && board[y + 1, x].color == color && board[y + 1, x].special == 0 && !board[y + 1, x].attacking)
            { // if theres enough room, the unit direcly below is a basic, the color matches, and isn't attacking
                if (board[y + 2, x].color == color && !board[y + 2, x].attacking)
                { // checks the color and attack of the unit 2 below
                    if (board[y + 2, x].special == 0)
                    { // if basic
                        return 1;
                    }
                    else if (board[y + 2, x].horz == 0 || (board[y, x + board[y + 2, x].horz].special == 0 && board[y, x + board[y + 2, x].horz].color == color && board[y + 1, x + board[y + 2, x].horz].special == 0 && board[y + 1, x + board[y + 2, x].horz].color == color))
                    { // if the two units to the side are basic and match color; assumes specials less than 0 don't have color other than black, and that the units to the side can't attack
                        return 2;
                    }
                }
            }
            return 0;
        }

        // checks if there's 3+ in a row horizontal; shouldn't assume unit at spot
        int CheckDefenseMatchLeft(string color, int y, int x)
        {
            int left = 0;

            if (x > 0 && board[y, x - 1].special == 0 && board[y, x - 1].color == color && !board[y, x - 1].attacking)
            {
                left++;
                if (x > 1 && board[y, x - 2].special == 0 && board[y, x - 2].color == color && !board[y, x - 2].attacking)
                {
                    left++;
                }
            }

            return left;
        }
        int CheckDefenseMatchRight(string color, int y, int x)
        {
            int right = 0;

            for (int i = width - 1; i > -1; i--)
            {
                if (x < i && board[y, x + (width - i)].special == 0 && board[y, x + (width - i)].color == color && !board[y, x + (width - i)].attacking)
                {
                    right++;
                }
                else
                {
                    i = -1;
                }
            }

            return right;
        }

        void AddDefence(int x, int num)
        {
            if (num == 0) return;

            int pos = height - 1;
            for (int y = height - 1; y > 0; y--)
            {
                if (board[y, x].special == -2)
                {
                    pos = y - 1;
                }
                else
                {
                    if (board[y, x].special == -1)
                    {
                        num--;
                        board[y, x] = UNIT_LISTS[chosenTheme][0][1].Clone();
                    }
                    y = 0;
                }
            }

            while (num > 0)
            {
                Up(pos, x, 1);
                if (num > 1)
                {
                    num -= 2;
                    board[pos, x] = UNIT_LISTS[chosenTheme][0][1].Clone();
                    pos--;
                }
                else
                {
                    num--;
                    board[pos, x] = UNIT_LISTS[chosenTheme][0][0].Clone();
                }
            }
        }

        void AddAttack(int x, int[] nums)
        {
            if (nums[1] == 0) return;

            string temp = "";
            if (nums[0] == 1)
            {
                temp = "red";
            }
            else if (nums[0] == 2)
            {
                temp = "green";
            }
            else if (nums[0] == 3)
            {
                temp = "blue";
            }

            int pos = height - 1;
            for (int y = height - 1; y > 0; y--)
            {
                if (board[y, x].special < 0)
                {
                    pos = y - 1;
                }
                else
                {
                    y = 0;
                }
            }

            if (nums[1] == 1)
            {
                if (board[pos, x].attacking && board[pos, x].colorCode == nums[0])
                {
                    Console.WriteLine("COMBO!");
                    board[pos, x].attack += UNIT_LISTS[chosenTheme][1][chosenBasicFormes[nums[0] - 1]].attack * 2 / 3;
                    board[pos - 1, x].attack += UNIT_LISTS[chosenTheme][1][chosenBasicFormes[nums[0] - 1]].attack * 2 / 3;
                    board[pos - 2, x].attack += UNIT_LISTS[chosenTheme][1][chosenBasicFormes[nums[0] - 1]].attack * 2 / 3;
                }
                else
                {
                    Up(pos, x, 3);
                    board[pos, x] = UNIT_LISTS[chosenTheme][1][chosenBasicFormes[nums[0] - 1]].Clone();
                    board[pos, x].color = temp;
                    board[pos, x].colorCode = nums[0];
                    board[pos, x].attacking = true;
                    board[pos - 1, x] = UNIT_LISTS[chosenTheme][1][chosenBasicFormes[nums[0] - 1]].Clone();
                    board[pos - 1, x].color = temp;
                    board[pos - 1, x].colorCode = nums[0];
                    board[pos - 1, x].attacking = true;
                    board[pos - 2, x] = UNIT_LISTS[chosenTheme][1][chosenBasicFormes[nums[0] - 1]].Clone();
                    board[pos - 2, x].color = temp;
                    board[pos - 2, x].colorCode = nums[0];
                    board[pos - 2, x].attacking = true;
                }
            }
            else
            {
                nums[1] -= 2;
                if (UNIT_LISTS[chosenTheme][chosenSpecials[nums[1]]][0].horz == 0)
                {
                    Up(pos, x, 2);
                    board[pos, x] = UNIT_LISTS[chosenTheme][chosenSpecials[nums[1]]][1].Clone();
                    board[pos, x].color = temp;
                    board[pos, x].colorCode = nums[0];
                    board[pos, x].attacking = true;
                    board[pos, x].cost += 2;
                    board[pos - 1, x] = UNIT_LISTS[chosenTheme][chosenSpecials[nums[1]]][0].Clone();
                    board[pos - 1, x].color = temp;
                    board[pos - 1, x].colorCode = nums[0];
                    board[pos - 1, x].attacking = true;
                    board[pos - 1, x].cost += 2;
                }
                else
                {
                    int pos2 = height - 1;
                    for (int y = height - 1; y > 0; y--)
                    {
                        if (board[y, x + 1].special < 0)
                        {
                            pos2 = y - 1;
                        }
                        else
                        {
                            y = 0;
                        }
                    }
                    int pos3 = Math.Min(pos, pos2);
                    Up(pos, x, 2 + Math.Abs(pos - pos3));
                    Up(pos2, x + 1, 2 + Math.Abs(pos2 - pos3));
                    board[pos3, x] = UNIT_LISTS[chosenTheme][chosenSpecials[nums[1]]][1].Clone();
                    board[pos3, x].color = temp;
                    board[pos3, x].colorCode = nums[0];
                    board[pos3, x].attacking = true;
                    board[pos3, x].cost += 4;
                    board[pos3 - 1, x] = UNIT_LISTS[chosenTheme][chosenSpecials[nums[1]]][0].Clone();
                    board[pos3 - 1, x].color = temp;
                    board[pos3 - 1, x].colorCode = nums[0];
                    board[pos3 - 1, x].attacking = true;
                    board[pos3 - 1, x].cost += 4;
                    board[pos3, x + 1] = UNIT_LISTS[chosenTheme][chosenSpecials[nums[1]]][3].Clone();
                    board[pos3, x + 1].color = temp;
                    board[pos3, x + 1].colorCode = nums[0];
                    board[pos3, x + 1].attacking = true;
                    board[pos3, x + 1].cost += 4;
                    board[pos3 - 1, x + 1] = UNIT_LISTS[chosenTheme][chosenSpecials[nums[1]]][2].Clone();
                    board[pos3 - 1, x + 1].color = temp;
                    board[pos3 - 1, x + 1].colorCode = nums[0];
                    board[pos3 - 1, x + 1].attacking = true;
                    board[pos3 - 1, x + 1].cost += 4;
                }
            }
        }

        public DataPacket TurnStart()
        {
            DataPacket data = new();
            for (int x = 0; x < width; x++)
            {
                for (int y = height - 1; y > -1; y--)
                {
                    // if (has special that triggers every turn)
                    if (board[y, x].attacking)
                    {
                        if (board[y, x].special == 0)
                        {
                            board[y, x].attack += board[y, x].growth;
                            board[y - 1, x].attack += board[y - 1, x].growth;
                            board[y - 2, x].attack += board[y - 2, x].growth;
                            board[y, x].speed--;
                            board[y - 1, x].speed--;
                            board[y - 2, x].speed--;

                            if (board[y, x].speed == 0)
                            {
                                data.attacks.Add([x, board[y, x].attack, 0]);
                                board[y, x] = BLANK_UNIT;
                                board[y - 1, x] = BLANK_UNIT;
                                board[y - 2, x] = BLANK_UNIT;
                                if (y - 3 > 0) Down(y - 3, x);
                                y++;
                            }
                            else
                            {
                                y -= 3;
                            }
                        }
                        else if (board[y, x].special == 1)
                        {
                            if (board[y, x].horz == 0)
                            {
                                board[y, x].attack += board[y, x].growth;
                                board[y - 1, x].attack += board[y - 1, x].growth;
                                board[y, x].speed--;
                                board[y - 1, x].speed--;

                                if (board[y, x].speed == 0)
                                {
                                    data.attacks.Add([x, board[y, x].attack, 0]);
                                    board[y, x] = BLANK_UNIT;
                                    board[y - 1, x] = BLANK_UNIT;
                                    if (y - 2 > 0) Down(y - 2, x);
                                    y++;
                                }
                                else
                                {
                                    y -= 2;
                                }
                            }
                            else
                            {
                                board[y, x].attack += board[y, x].growth;
                                board[y - 1, x].attack += board[y - 1, x].growth;
                                board[y, x + 1].attack += board[y, x].growth;
                                board[y - 1, x + 1].attack += board[y - 1, x].growth;
                                board[y, x].speed--;
                                board[y - 1, x].speed--;
                                board[y, x + 1].speed--;
                                board[y - 1, x + 1].speed--;

                                if (board[y, x].speed == 0)
                                {
                                    data.attacks.Add([x, board[y, x].attack, 1]);
                                    board[y, x] = BLANK_UNIT;
                                    board[y - 1, x] = BLANK_UNIT;
                                    board[y, x + 1] = BLANK_UNIT;
                                    board[y - 1, x + 1] = BLANK_UNIT;
                                    if (y - 2 > 0)
                                    {
                                        Down(y - 2, x);
                                        Down(y - 2, x + 1);
                                    }
                                    y++;
                                }
                                else
                                {
                                    y -= 2;
                                }
                            }
                        }
                    }
                    else if (board[y, x].special >= 0)
                    {
                        y = -1;
                    }
                }
            }
            return data;
        }

        // !IMPORTANT! 
        // can be optimzed, repeats the same lines of code a ton of times
        // i'm too tired rn, and wanna move on to Project: Citrus
        // !IMPORTANT!
        public int[] TurnEnd(DataPacket data)
        { // i'm so sorry
            int[] damCost = [0, 0];
            foreach (int[] attack in data.attacks)
            {
                if (attack[2] == 0)
                {
                    for (int y = height - 1; y > -1; y--)
                    {
                        if (!board[y, attack[0]].attacking)
                        {
                            if (board[y, attack[0]].special < 1)
                            {
                                damCost[1] += board[y, attack[0]].cost;
                                if (board[y, attack[0]].health >= attack[1])
                                {
                                    board[y, attack[0]] = BLANK_UNIT;
                                    if (y > 0)
                                    {
                                        Down(y - 1, attack[0]);
                                    }
                                    y = -1;
                                }
                                else
                                {
                                    attack[1] -= board[y, attack[0]].health;
                                    board[y, attack[0]] = BLANK_UNIT;
                                    if (y == 0)
                                    {
                                        damCost[0] += attack[1];
                                    }
                                }
                            }
                            else if (board[y, attack[0]].special != 27)
                            { // if special not do anything weird with being attacked while not attacking
                                damCost[1] += board[y, attack[0]].cost;
                                if (board[y, attack[0]].health >= attack[1])
                                {
                                    if (board[y, attack[0]].horz == 0)
                                    {
                                        board[y, attack[0]] = BLANK_UNIT;
                                        board[y - 1, attack[0]] = BLANK_UNIT;
                                        if (y > 1)
                                        {
                                            Down(y - 2, attack[0]);
                                        }
                                    }
                                    else
                                    {
                                        int horz = board[y,attack[0]].horz;
                                        board[y, attack[0] + horz] = BLANK_UNIT;
                                        board[y - 1, attack[0] + horz] = BLANK_UNIT;
                                        board[y, attack[0]] = BLANK_UNIT;
                                        board[y - 1, attack[0]] = BLANK_UNIT;
                                        if (y > 1)
                                        {
                                            Down(y - 2, attack[0]);
                                            Down(y - 2, attack[0] + horz);
                                        }
                                    }
                                    y = -1;
                                }
                                else
                                {
                                    attack[1] -= board[y, attack[0]].health;
                                    if (board[y, attack[0]].horz == 0)
                                    {
                                        board[y, attack[0]] = BLANK_UNIT;
                                        board[y - 1, attack[0]] = BLANK_UNIT;
                                    }
                                    else
                                    {
                                        int horz = board[y,attack[0]].horz;
                                        board[y, attack[0]] = BLANK_UNIT;
                                        board[y - 1, attack[0]] = BLANK_UNIT;
                                        board[y, attack[0] + horz] = BLANK_UNIT;
                                        board[y - 1, attack[0] + horz] = BLANK_UNIT;
                                        if (y > 1)
                                        {
                                            Down(y - 2, attack[0] + horz); // asumes no other wide unit
                                        }
                                    }

                                    if (y == 1)
                                    {
                                        damCost[0] += attack[1];
                                    }
                                }
                            }
                        }
                        else if (board[y, attack[0]].special != 2)
                        { // if special not do anything weird when attacked while attacking
                            if (board[y, attack[0]].attack > attack[1])
                            {
                                board[y, attack[0]].attack -= attack[1];
                                board[y - 1, attack[0]].attack -= attack[1];
                                if (board[y, attack[0]].special == 0)
                                {
                                    board[y - 2, attack[0]].attack -= attack[1];
                                }
                                else if (board[y, attack[0]].horz != 0)
                                {
                                    board[y, attack[0] + board[y, attack[0]].horz].attack -= attack[1];
                                    board[y - 1, attack[0] + board[y, attack[0]].horz].attack -= attack[1];
                                }
                                y = -1;
                            }
                            else
                            {
                                attack[1] -= board[y, attack[0]].attack;

                                if (board[y, attack[0]].special == 0)
                                {
                                    damCost[1] += 3;
                                    board[y, attack[0]] = BLANK_UNIT;
                                    board[y - 1, attack[0]] = BLANK_UNIT;
                                    board[y - 2, attack[0]] = BLANK_UNIT;
                                    if (attack[1] != 0)
                                    {
                                        if (y > 2)
                                        {
                                            Down(y - 3, attack[0]);
                                        }
                                        else if (y == 2)
                                        {
                                            damCost[0] += attack[1];
                                            y = -1;
                                        }
                                    }
                                }
                                else if (board[y, attack[0]].horz == 0)
                                {
                                    damCost[1] += board[y, attack[0]].cost + 2;
                                    board[y, attack[0]] = BLANK_UNIT;
                                    board[y - 1, attack[0]] = BLANK_UNIT;
                                    if (attack[1] != 0)
                                    {
                                        if (y > 1)
                                        {
                                            Down(y - 2, attack[0]);
                                        }
                                        else if (y == 1)
                                        {
                                            damCost[0] += attack[1];
                                            y = -1;
                                        }
                                    }
                                }
                                else
                                {
                                    damCost[1] += board[y, attack[0]].cost + 4;
                                    int horz = board[y,attack[0]].horz;
                                    board[y, attack[0]] = BLANK_UNIT;
                                    board[y - 1, attack[0]] = BLANK_UNIT;
                                    board[y, attack[0] + horz] = BLANK_UNIT;
                                    board[y - 1, attack[0] + horz] = BLANK_UNIT;
                                    if (attack[1] != 0)
                                    {
                                        if (y > 1)
                                        {
                                            Down(y - 2, attack[0]);
                                            Down(y - 2, attack[0] + horz);
                                        }
                                        else if (y == 1)
                                        {
                                            damCost[0] += attack[1];
                                            y = -1;
                                        }
                                    }
                                }

                                if (attack[1] == 0)
                                {
                                    y = -1;
                                }
                            }
                        }
                        else if (board[y, attack[0]].special == 2) { }
                    }
                }
                else if (attack[2] == 1)
                { // wide unit; assumes the left side of the two is called
                    for (int y = height - 1; y > -1; y--)
                    {
                        if (board[y, attack[0]].horz != 1)
                        {
                            if (!board[y, attack[0]].attacking && !board[y, attack[0] + 1].attacking)
                            {
                                damCost[1] += board[y, attack[0]].cost + board[y, attack[0] + 1].cost;
                                int sum = board[y, attack[0]].health + board[y, attack[0] + 1].health;
                                if (sum >= attack[1])
                                {
                                    if (board[y, attack[0]].special <= 0)
                                    {
                                        board[y, attack[0]] = BLANK_UNIT;
                                        if (y > 0)
                                        {
                                            Down(y - 1, attack[0]);
                                        }
                                    }
                                    else if (board[y, attack[0]].horz == 0)
                                    {
                                        board[y, attack[0]] = BLANK_UNIT;
                                        board[y - 1, attack[0]] = BLANK_UNIT;
                                        if (y > 1)
                                        {
                                            Down(y - 2, attack[0]);
                                        }
                                    }
                                    else
                                    {
                                        board[y, attack[0]] = BLANK_UNIT;
                                        board[y - 1, attack[0]] = BLANK_UNIT;
                                        board[y, attack[0] - 1] = BLANK_UNIT;
                                        board[y - 1, attack[0] - 1] = BLANK_UNIT;
                                        if (y > 1)
                                        {
                                            Down(y - 2, attack[0]);
                                            Down(y - 2, attack[0] - 1);
                                        }
                                    }

                                    if (board[y, attack[0] + 1].special <= 0)
                                    {
                                        board[y, attack[0] + 1] = BLANK_UNIT;
                                        if (y > 0)
                                        {
                                            Down(y - 1, attack[0] + 1);
                                        }
                                    }
                                    else if (board[y, attack[0] + 1].horz == 0)
                                    {
                                        board[y, attack[0] + 1] = BLANK_UNIT;
                                        board[y - 1, attack[0] + 1] = BLANK_UNIT;
                                        if (y > 1)
                                        {
                                            Down(y - 2, attack[0] + 1);
                                        }
                                    }
                                    else
                                    {
                                        board[y, attack[0] + 1] = BLANK_UNIT;
                                        board[y - 1, attack[0] + 1] = BLANK_UNIT;
                                        board[y, attack[0] + 2] = BLANK_UNIT;
                                        board[y - 1, attack[0] + 2] = BLANK_UNIT;
                                        if (y > 1)
                                        {
                                            Down(y - 2, attack[0] + 1);
                                            Down(y - 2, attack[0] + 2);
                                        }
                                    }
                                    y = -1;
                                }
                                else
                                {
                                    attack[1] -= sum;
                                    if (y == 0)
                                    {
                                        board[y, attack[0]] = BLANK_UNIT;
                                        board[y, attack[0] + 1] = BLANK_UNIT;
                                        damCost[0] += attack[1];
                                    }
                                    else
                                    {
                                        if (board[y, attack[0]].special <= 0)
                                        {
                                            board[y, attack[0]] = BLANK_UNIT;
                                        }
                                        else if (board[y, attack[0]].horz == 0)
                                        {
                                            board[y, attack[0]] = BLANK_UNIT;
                                            board[y - 1, attack[0]] = BLANK_UNIT;
                                        }
                                        else
                                        {
                                            board[y, attack[0]] = BLANK_UNIT;
                                            board[y - 1, attack[0]] = BLANK_UNIT;
                                            board[y, attack[0] - 1] = BLANK_UNIT;
                                            board[y - 1, attack[0] - 1] = BLANK_UNIT;
                                            if (y > 1)
                                            {
                                                Down(y - 2, attack[0] - 1);
                                            }
                                        }

                                        if (board[y, attack[0] + 1].special <= 0)
                                        {
                                            board[y, attack[0] + 1] = BLANK_UNIT;
                                        }
                                        else if (board[y, attack[0] + 1].horz == 0)
                                        {
                                            board[y, attack[0] + 1] = BLANK_UNIT;
                                            board[y - 1, attack[0] + 1] = BLANK_UNIT;
                                        }
                                        else
                                        {
                                            board[y, attack[0] + 1] = BLANK_UNIT;
                                            board[y - 1, attack[0] + 1] = BLANK_UNIT;
                                            board[y, attack[0] + 2] = BLANK_UNIT;
                                            board[y - 1, attack[0] + 2] = BLANK_UNIT;
                                            if (y > 1)
                                            {
                                                Down(y - 2, attack[0] + 2);
                                            }
                                        }
                                    }
                                }
                            }
                            else if (board[y, attack[0]].attacking && !board[y, attack[0] + 1].attacking)
                            {
                                damCost[1] += board[y, attack[0] + 1].cost;
                                if (board[y, attack[0]].special != 2)
                                { // if special not do something strange when attacked while attacking
                                    int sum = board[y, attack[0]].attack + board[y, attack[0] + 1].health;
                                    if (sum < attack[1])
                                    {
                                        attack[1] -= sum;
                                        damCost[1] += board[y, attack[0]].cost;

                                        if (board[y, attack[0]].special == 0)
                                        {
                                            damCost[1] += 2;
                                            board[y, attack[0]] = BLANK_UNIT;
                                            board[y - 1, attack[0]] = BLANK_UNIT;
                                            board[y - 2, attack[0]] = BLANK_UNIT;
                                        }
                                        else if (board[y, attack[0]].horz == 0)
                                        {
                                            damCost[1] += 2;
                                            board[y, attack[0]] = BLANK_UNIT;
                                            board[y - 1, attack[0]] = BLANK_UNIT;
                                        }
                                        else
                                        {
                                            damCost[1] += 4;
                                            board[y, attack[0]] = BLANK_UNIT;
                                            board[y - 1, attack[0]] = BLANK_UNIT;
                                            board[y, attack[0] - 1] = BLANK_UNIT;
                                            board[y - 1, attack[0] - 1] = BLANK_UNIT;
                                            if (y > 1)
                                            {
                                                Down(y - 2, attack[0] - 1);
                                            }
                                        }

                                        if (board[y, attack[0] + 1].special <= 0)
                                        {
                                            board[y, attack[0] + 1] = BLANK_UNIT;
                                        }
                                        else if (board[y, attack[0] + 1].horz == 0)
                                        {
                                            board[y, attack[0] + 1] = BLANK_UNIT;
                                            board[y - 1, attack[0] + 1] = BLANK_UNIT;
                                        }
                                        else
                                        {
                                            board[y, attack[0] + 1] = BLANK_UNIT;
                                            board[y - 1, attack[0] + 1] = BLANK_UNIT;
                                            board[y, attack[0] + 2] = BLANK_UNIT;
                                            board[y - 1, attack[0] + 2] = BLANK_UNIT;
                                            if (y > 1)
                                            {
                                                Down(y - 2, attack[0] + 2);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (board[y, attack[0]].attack <= attack[1])
                                        {
                                            damCost[1] += board[y, attack[0]].cost;

                                            if (board[y, attack[0]].special == 0)
                                            {
                                                damCost[1] += 2;
                                                board[y, attack[0]] = BLANK_UNIT;
                                                board[y - 1, attack[0]] = BLANK_UNIT;
                                                board[y - 2, attack[0]] = BLANK_UNIT;
                                                if (y > 2)
                                                {
                                                    Down(y - 3, attack[0]);
                                                }
                                            }
                                            else if (board[y, attack[0]].horz == 0)
                                            {
                                                damCost[1] += 2;
                                                board[y, attack[0]] = BLANK_UNIT;
                                                board[y - 1, attack[0]] = BLANK_UNIT;
                                                if (y > 1)
                                                {
                                                    Down(y - 2, attack[0]);
                                                }
                                            }
                                            else
                                            {
                                                damCost[1] += 4;
                                                board[y, attack[0]] = BLANK_UNIT;
                                                board[y - 1, attack[0]] = BLANK_UNIT;
                                                board[y, attack[0] - 1] = BLANK_UNIT;
                                                board[y - 1, attack[0] - 1] = BLANK_UNIT;
                                                if (y > 1)
                                                {
                                                    Down(y - 2, attack[0]);
                                                    Down(y - 2, attack[0] - 1);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (board[y, attack[0]].special == 0)
                                            {
                                                board[y, attack[0]].attack -= attack[1];
                                                board[y - 1, attack[0]].attack -= attack[1];
                                                board[y - 2, attack[0]].attack -= attack[1];
                                            }
                                            else if (board[y, attack[0]].horz == 0)
                                            {
                                                board[y, attack[0]].attack -= attack[1];
                                                board[y - 1, attack[0]].attack -= attack[1];
                                            }
                                            else
                                            {
                                                board[y, attack[0]].attack -= attack[1];
                                                board[y - 1, attack[0]].attack -= attack[1];
                                                board[y, attack[0] - 1].attack -= attack[1];
                                                board[y - 1, attack[0] - 1].attack -= attack[1];
                                            }
                                        }

                                        if (board[y, attack[0] + 1].special <= 0)
                                        {
                                            board[y, attack[0] + 1] = BLANK_UNIT;
                                        }
                                        else if (board[y, attack[0] + 1].horz == 0)
                                        {
                                            board[y, attack[0] + 1] = BLANK_UNIT;
                                            board[y - 1, attack[0] + 1] = BLANK_UNIT;
                                        }
                                        else
                                        {
                                            board[y, attack[0] + 1] = BLANK_UNIT;
                                            board[y - 1, attack[0] + 1] = BLANK_UNIT;
                                            board[y, attack[0] + 2] = BLANK_UNIT;
                                            board[y - 1, attack[0] + 2] = BLANK_UNIT;
                                            if (y > 1)
                                            {
                                                Down(y - 2, attack[0] + 2);
                                            }
                                        }
                                        y = -1;
                                    }
                                }
                                else { }
                            }
                            else if (!board[y, attack[0]].attacking && board[y, attack[0] + 1].attacking)
                            {
                                damCost[1] += board[y, attack[0]].cost;
                                if (board[y, attack[0] + 1].special != 2)
                                { // if special not do something strange when attacked while attacking
                                    int sum = board[y, attack[0]].health + board[y, attack[0] + 1].attack;
                                    if (sum < attack[1])
                                    {
                                        attack[1] -= sum;
                                        damCost[1] += board[y, attack[0] + 1].cost;

                                        if (board[y, attack[0]].special <= 0)
                                        {
                                            board[y, attack[0]] = BLANK_UNIT;
                                        }
                                        else if (board[y, attack[0]].horz == 0)
                                        {
                                            board[y, attack[0]] = BLANK_UNIT;
                                            board[y - 1, attack[0]] = BLANK_UNIT;
                                        }
                                        else
                                        {
                                            board[y, attack[0]] = BLANK_UNIT;
                                            board[y - 1, attack[0]] = BLANK_UNIT;
                                            board[y, attack[0] - 1] = BLANK_UNIT;
                                            board[y - 1, attack[0] - 1] = BLANK_UNIT;
                                            if (y > 1)
                                            {
                                                Down(y - 2, attack[0] - 1);
                                            }
                                        }

                                        if (board[y, attack[0] + 1].special == 0)
                                        {
                                            damCost[1] += 2;
                                            board[y, attack[0] + 1] = BLANK_UNIT;
                                            board[y - 1, attack[0] + 1] = BLANK_UNIT;
                                            board[y - 2, attack[0] + 1] = BLANK_UNIT;
                                        }
                                        else if (board[y, attack[0] + 1].horz == 0)
                                        {
                                            damCost[1] += 2;
                                            board[y, attack[0] + 1] = BLANK_UNIT;
                                            board[y - 1, attack[0] + 1] = BLANK_UNIT;
                                        }
                                        else
                                        {
                                            damCost[1] += 4;
                                            board[y, attack[0] + 1] = BLANK_UNIT;
                                            board[y - 1, attack[0] + 1] = BLANK_UNIT;
                                            board[y, attack[0] + 2] = BLANK_UNIT;
                                            board[y - 1, attack[0] + 2] = BLANK_UNIT;
                                            if (y > 1)
                                            {
                                                Down(y - 2, attack[0] + 2);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (board[y, attack[0]].special <= 0)
                                        {
                                            board[y, attack[0]] = BLANK_UNIT;
                                        }
                                        else if (board[y, attack[0]].horz == 0)
                                        {
                                            board[y, attack[0]] = BLANK_UNIT;
                                            board[y - 1, attack[0]] = BLANK_UNIT;
                                        }
                                        else
                                        {
                                            board[y, attack[0]] = BLANK_UNIT;
                                            board[y - 1, attack[0]] = BLANK_UNIT;
                                            board[y, attack[0] - 1] = BLANK_UNIT;
                                            board[y - 1, attack[0] - 1] = BLANK_UNIT;
                                            if (y > 1)
                                            {
                                                Down(y - 2, attack[0] - 1);
                                            }
                                        }

                                        if (board[y, attack[0] + 1].attack <= attack[1])
                                        {
                                            damCost[1] += board[y, attack[0] + 1].cost;

                                            if (board[y, attack[0] + 1].special == 0)
                                            {
                                                damCost[1] += 2;
                                                board[y, attack[0] + 1] = BLANK_UNIT;
                                                board[y - 1, attack[0] + 1] = BLANK_UNIT;
                                                board[y - 2, attack[0] + 1] = BLANK_UNIT;
                                                if (y > 2)
                                                {
                                                    Down(y - 3, attack[0] + 1);
                                                }
                                            }
                                            else if (board[y, attack[0] + 1].horz == 0)
                                            {
                                                damCost[1] += 2;
                                                board[y, attack[0] + 1] = BLANK_UNIT;
                                                board[y - 1, attack[0] + 1] = BLANK_UNIT;
                                                if (y > 1)
                                                {
                                                    Down(y - 2, attack[0] + 1);
                                                }
                                            }
                                            else
                                            {
                                                damCost[1] += 4;
                                                board[y, attack[0] + 1] = BLANK_UNIT;
                                                board[y - 1, attack[0] + 1] = BLANK_UNIT;
                                                board[y, attack[0] + 2] = BLANK_UNIT;
                                                board[y - 1, attack[0] + 2] = BLANK_UNIT;
                                                if (y > 1)
                                                {
                                                    Down(y - 2, attack[0] + 1);
                                                    Down(y - 2, attack[0] + 2);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (board[y, attack[0] + 1].special == 0)
                                            {
                                                board[y, attack[0] + 1].attack -= attack[1];
                                                board[y - 1, attack[0] + 1].attack -= attack[1];
                                                board[y - 2, attack[0] + 1].attack -= attack[1];
                                            }
                                            else if (board[y, attack[0] + 1].horz == 0)
                                            {
                                                board[y, attack[0] + 1].attack -= attack[1];
                                                board[y - 1, attack[0] + 1].attack -= attack[1];
                                            }
                                            else
                                            {
                                                board[y, attack[0] + 1].attack -= attack[1];
                                                board[y - 1, attack[0] + 1].attack -= attack[1];
                                                board[y, attack[0] + 2].attack -= attack[1];
                                                board[y - 1, attack[0] + 2].attack -= attack[1];
                                            }
                                        }
                                        y = -1;
                                    }
                                }
                                else { }
                            }
                            else if (board[y, attack[0]].attacking && board[y, attack[0] + 1].attacking)
                            {
                                int sum = board[y, attack[0]].attack + board[y, attack[0] + 1].attack;
                                if (sum >= attack[1])
                                {
                                    if (board[y, attack[0]].attack <= attack[1])
                                    {
                                        damCost[1] += board[y, attack[0]].cost;

                                        if (board[y, attack[0]].special == 0)
                                        {
                                            damCost[1] += 2;
                                            board[y, attack[0]] = BLANK_UNIT;
                                            board[y - 1, attack[0]] = BLANK_UNIT;
                                            board[y - 2, attack[0]] = BLANK_UNIT;
                                            if (y > 2)
                                            {
                                                Down(y - 3, attack[0]);
                                            }
                                        }
                                        else if (board[y, attack[0]].horz == 0)
                                        {
                                            damCost[1] += 2;
                                            board[y, attack[0]] = BLANK_UNIT;
                                            board[y - 1, attack[0]] = BLANK_UNIT;
                                            if (y > 1)
                                            {
                                                Down(y - 2, attack[0] + 1);
                                            }
                                        }
                                        else
                                        {
                                            damCost[1] += 4;
                                            board[y, attack[0]] = BLANK_UNIT;
                                            board[y - 1, attack[0]] = BLANK_UNIT;
                                            board[y, attack[0] - 1] = BLANK_UNIT;
                                            board[y - 1, attack[0] - 1] = BLANK_UNIT;
                                            if (y > 1)
                                            {
                                                Down(y - 2, attack[0]);
                                                Down(y - 2, attack[0] - 1);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (board[y, attack[0]].special == 0)
                                        {
                                            board[y, attack[0]].attack -= attack[1];
                                            board[y - 1, attack[0]].attack -= attack[1];
                                            board[y - 2, attack[0]].attack -= attack[1];
                                        }
                                        else if (board[y, attack[0]].horz == 0)
                                        {
                                            board[y, attack[0]].attack -= attack[1];
                                            board[y - 1, attack[0]].attack -= attack[1];
                                        }
                                        else
                                        {
                                            board[y, attack[0]].attack -= attack[1];
                                            board[y - 1, attack[0]].attack -= attack[1];
                                            board[y, attack[0] - 1].attack -= attack[1];
                                            board[y - 1, attack[0] - 1].attack -= attack[1];
                                        }
                                    }

                                    if (board[y, attack[0] + 1].attack <= attack[1])
                                    {
                                        damCost[1] += board[y, attack[0] + 1].cost;

                                        if (board[y, attack[0] + 1].special == 0)
                                        {
                                            damCost[1] += 2;
                                            board[y, attack[0] + 1] = BLANK_UNIT;
                                            board[y - 1, attack[0] + 1] = BLANK_UNIT;
                                            board[y - 2, attack[0] + 1] = BLANK_UNIT;
                                            if (y > 2)
                                            {
                                                Down(y - 3, attack[0] + 1);
                                            }
                                        }
                                        else if (board[y, attack[0] + 1].horz == 0)
                                        {
                                            damCost[1] += 2;
                                            board[y, attack[0] + 1] = BLANK_UNIT;
                                            board[y - 1, attack[0] + 1] = BLANK_UNIT;
                                            if (y > 1)
                                            {
                                                Down(y - 2, attack[0] + 1);
                                            }
                                        }
                                        else
                                        {
                                            damCost[1] += 4;
                                            board[y, attack[0] + 1] = BLANK_UNIT;
                                            board[y - 1, attack[0] + 1] = BLANK_UNIT;
                                            board[y, attack[0] + 2] = BLANK_UNIT;
                                            board[y - 1, attack[0] + 2] = BLANK_UNIT;
                                            if (y > 1)
                                            {
                                                Down(y - 2, attack[0] + 1);
                                                Down(y - 2, attack[0] + 2);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (board[y, attack[0] + 1].special == 0)
                                        {
                                            board[y, attack[0] + 1].attack -= attack[1];
                                            board[y - 1, attack[0] + 1].attack -= attack[1];
                                            board[y - 2, attack[0] + 1].attack -= attack[1];
                                        }
                                        else if (board[y, attack[0] + 1].horz == 0)
                                        {
                                            board[y, attack[0] + 1].attack -= attack[1];
                                            board[y - 1, attack[0] + 1].attack -= attack[1];
                                        }
                                        else
                                        {
                                            board[y, attack[0] + 1].attack -= attack[1];
                                            board[y - 1, attack[0] + 1].attack -= attack[1];
                                            board[y, attack[0] + 2].attack -= attack[1];
                                            board[y - 1, attack[0] + 2].attack -= attack[1];
                                        }
                                    }
                                    y = -1;
                                }
                            }
                            else
                            {
                                throw new Exception("not: neither attack, left attack, right attack, or both attack. how?");
                            }
                        }
                        else // attacking a wide unit whose width matches
                        {
                            if (!board[y, attack[0]].attacking)
                            {
                                damCost[1] += board[y, attack[0]].cost;
                                attack[1] -= board[y, attack[0]].health;

                                board[y, attack[0]] = BLANK_UNIT;
                                board[y - 1, attack[0]] = BLANK_UNIT;
                                board[y, attack[0] + 1] = BLANK_UNIT;
                                board[y - 1, attack[0] + 1] = BLANK_UNIT;

                                if (attack[1] <= 0)
                                {
                                    if (y > 1)
                                    {
                                        Down(y - 2, attack[0]);
                                        Down(y - 2, attack[0] + 1);
                                    }
                                    y = -1;
                                }
                                else if (y == 1)
                                {
                                    damCost[0] += attack[1];
                                    y = -1;
                                }
                            }
                            else
                            {
                                if (board[y, attack[0]].attack > attack[1])
                                {
                                    board[y, attack[0]].attack -= attack[1];
                                    board[y - 1, attack[0]].attack -= attack[1];
                                    board[y, attack[0] + 1].attack -= attack[1];
                                    board[y - 1, attack[0] + 1].attack -= attack[1];
                                    y = -1;
                                }
                                else
                                {
                                    attack[1] -= board[y, attack[0]].attack;
                                    damCost[1] += board[y, attack[0]].cost + 4;

                                    board[y, attack[0]] = BLANK_UNIT;
                                    board[y - 1, attack[0]] = BLANK_UNIT;
                                    board[y, attack[0] + 1] = BLANK_UNIT;
                                    board[y - 1, attack[0] + 1] = BLANK_UNIT;

                                    if (attack[1] == 0)
                                    {
                                        if (y > 1)
                                        {
                                            Down(y - 2, attack[0]);
                                            Down(y - 2, attack[0] + 1);
                                        }
                                        y = -1;
                                    }
                                }
                            }
                        }
                    }
                }
                else if (attack[2] == 2) { }
                else if (attack[2] == 3) { }
            }
            return damCost;
        }
    }

    // handles individual units
    class Unit
    {
        public string name, color;
        public int colorCode; // debug & pre-devcade demo use only
        public char symbol; // debug & pre-devcade demo use only
        /* public __ sprite; */
        public bool attacking, matchDefend;
        public int matchAttack, matchAttackID;
        public int health, attack, growth, speed, cost;
        public int vert, horz; // for use in speical unit sizes
        public int special;
        // -2 = defense teir 2
        // -1 = defense teir 1
        // 0 = basic unit & blank
        // 1+ = special unit
        // 1 = special unit without bonus effect 
        // 2 = double attacking health
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
            matchDefend = false;
            matchAttack = 0;
            matchAttackID = 0;
        }

        public Unit Clone()
        {
            return (Unit)MemberwiseClone();
        }
    }

    // data that passes between boards  on turn start/end
    class DataPacket
    {
        // column, damage, special
        // special: 0 = normal, 1 = wide
        public List<int[]> attacks = [];
        public DataPacket()
        {
        }
    }
}