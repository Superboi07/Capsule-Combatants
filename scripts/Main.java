import java.util.ArrayList;
import java.util.Scanner;

import java.util.InputMismatchException;
import java.util.Random;

class Main {

    static Scanner scan = new Scanner(System.in);
    static Random rand = new Random();

    static final int BOARD_WIDTH = 7;
    static final int BOARD_HEIGHT = 6;
    static Board[] boards = new Board[] {new Board(0, BOARD_WIDTH,BOARD_HEIGHT), new Board(1, BOARD_WIDTH,BOARD_HEIGHT)};

    static final int START_POINTS = 35;
    static final int START_HEALTH = 100;
    static final int START_ACTS = 5;

    static int[] playerPoints = new int[] { START_POINTS, START_POINTS };
    static int[] playerHealths = new int[] { START_HEALTH, START_HEALTH };
    static int[] playerActs = new int[] { START_ACTS, START_ACTS };
    static int[][] playerSpecialUnitCount = new int[][] { { 0, 0 }, { 0, 0 } };

    static ArrayList<Unit> unitList = new ArrayList<Unit>();
    static Unit[][] actUnitTypes = new Unit[2][5];

    static boolean reinforcing = false;

    public static void main(String[] args) throws CloneNotSupportedException {
        // System.out.println(ANSI_RED + "This text is red!" + ANSI_RESET);

        makeUnitList();
        chooseUnits();
        // chooseUnits(0);
        // chooseUnits(1);

        reinforce(0);
        printBoard();

        int temp1 = scan.nextInt();
        int temp2 = scan.nextInt();
        boards[0].removeUnit(temp1, temp2);
        printBoard();
    }

    static void printBoard() {
        for (int y = 0; y < 6; y++) {
            for (int x = 0; x < 7; x++) {
                System.out.print(boards[0].board[y][x].color + boards[0].board[y][x].icon + Unit.ANSI_RESET);
            }
            System.out.print('|');
            for (int x = 6; x > -1; x--) {
                System.out.print(boards[1].board[y][x].color + boards[1].board[y][x].icon + Unit.ANSI_RESET);
            }
            System.out.println();
        }
        System.out.println();
    }

    static void makeUnitList() {
        unitList.add(new Unit("barrier 1", 'x', 4, 0, 0, 0, 1, 1, 0, -1));
        unitList.get(0).color = Unit.ANSI_BLACK;
        unitList.add(new Unit("barrier 2", 'X', 9, 0, 0, 0, 1, 1, 0, -2));
        unitList.get(1).color = Unit.ANSI_BLACK;
        unitList.add(new Unit("demo weak", 'u', 2, 5, 5, 1, 1, 1, 1, 0));
        unitList.add(new Unit("demo strong", 'U', 3, 8, 6, 2, 1, 1, 1, 0));
        unitList.add(new Unit("demo special 1", 's', 7, 12, 10, 2, 2, 1, 4, 1));
        unitList.add(new Unit("demo special 2", 'S', 15, 18, 10, 3, 2, 2, 7, 2));
    }

    static void chooseUnits() throws CloneNotSupportedException {
        for (int i = 0; i < 2; i++) {
            for (int j = 0; j < 3; j++) {
                actUnitTypes[i][j] = (Unit) unitList.get(2).clone();
                actUnitTypes[i][j].color = Unit.COLORS[j][1];
            }
            actUnitTypes[i][3] = (Unit) unitList.get(4).clone();
            actUnitTypes[i][4] = (Unit) unitList.get(5).clone();
        }
    }

    static void chooseUnits(int player) throws CloneNotSupportedException {
        int theme = 0;
        int choice = -1;
        boolean loop = true;
        System.out.println("Player " + player + ":");
        // System.out.println("choose theme");
        if (theme == 0) {
            for (int i = 0; i < 3; i++) {
                while (loop) {
                    try {
                        System.out.println(
                                "choose basic " + Unit.COLORS[i][0] + " unit " + Unit.ANSI_RESET + ": [1] " + unitList.get(2).name
                                        + ", [2] " + unitList.get(3).name);
                        choice = scan.nextInt();
                        if (choice == 1) {
                            actUnitTypes[player][i] = (Unit) unitList.get(2).clone();
                            loop = false;
                        } else if (choice == 2) {
                            actUnitTypes[player][i] = (Unit) unitList.get(3).clone();
                            loop = false;
                        }
                    } catch (InputMismatchException e) {
                        scan.nextLine();
                    }
                }
                actUnitTypes[player][i].color = Unit.COLORS[i][1];
                loop = true;
            }
            actUnitTypes[player][3] = (Unit) unitList.get(4).clone();
            actUnitTypes[player][4] = (Unit) unitList.get(5).clone();
        }
    }

    static void reinforce(int player) throws CloneNotSupportedException {
        reinforcing = true;
        Unit tempUnit;
        int tempRand;
        while (playerPoints[player] > 0) {
            tempUnit = null;
            tempRand = rand.nextInt(100);
            int basicUnitOdds = 75; // should be divisible by 3
            int special1Odds = 15; // special 2 odds are remainder
            if (tempRand < basicUnitOdds / 3) {
                tempUnit = (Unit) actUnitTypes[player][0].clone();
            } else if (tempRand < basicUnitOdds * 2 / 3) {
                tempUnit = (Unit) actUnitTypes[player][1].clone();
            } else if (tempRand < basicUnitOdds) {
                tempUnit = (Unit) actUnitTypes[player][2].clone();
            } else if (tempRand < basicUnitOdds + special1Odds) {
                if (playerSpecialUnitCount[player][0] < actUnitTypes[player][3].max
                        && playerPoints[player] >= actUnitTypes[player][3].cost) {
                    playerSpecialUnitCount[player][0]++;
                    tempUnit = (Unit) actUnitTypes[player][3].clone();
                    tempRand = rand.nextInt(3);
                    tempUnit.color = Unit.COLORS[tempRand][1];
                }
            } else {
                if (playerSpecialUnitCount[player][1] < actUnitTypes[player][4].max
                        && playerPoints[player] >= actUnitTypes[player][4].cost) {
                    playerSpecialUnitCount[player][1]++;
                    tempUnit = (Unit) actUnitTypes[player][4].clone();
                    tempRand = rand.nextInt(3);
                    tempUnit.color = Unit.COLORS[tempRand][1];
                }
            }
            if (tempUnit != null && boards[player].addUnit(tempUnit)) {
                playerPoints[player] -= tempUnit.cost;
            }
        }
        reinforcing = false;
    }
}