import java.util.ArrayList;
import java.util.Scanner;

import javax.swing.plaf.basic.BasicInternalFrameTitlePane.SystemMenuBar;

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

    /*

    static boolean placeUnit(int player, Unit unit) {
        ArrayList<Integer> open = new ArrayList<Integer>();
        for (int i = 0; i < 7 - unit.height; i++) {
            if (checkSpot(player, unit, i, unit.length - 1)) {
                open.add(i);
            }
        }
        if (open.size() == 0) {
            System.out.println("not placed");
            return false;
        }
        int tempRand = open.get(rand.nextInt(open.size()));
        try {
            if (!findSpot(player, unit, tempRand, unit.length)) {
                if (placeUnit(player, unit, tempRand, unit.length - 1)) {
                }
            }
        } catch (NullPointerException e) {
            return false;
        }
        return true;
    }

    static Boolean findSpot(int player, Unit unit, int row, int column) {
        if (column < 7 && checkSpot(player, unit, row, column)) {
            try {
                if (!findSpot(player, unit, row, column + 1)) {
                    if (placeUnit(player, unit, row, column)) {
                    }
                }
                return true;
            } catch (NullPointerException e) {
                return null;
            }
        } else {
            return false;
        }
    }

    static boolean checkSpot(int player, Unit unit, int row, int column) {
        for (int i = 0; i < unit.length; i++) {
            for (int j = 0; j < unit.height; j++) {
                if (board[player][row + j][column - i].name != null) {
                    return false;
                }
            }
        }
        return true;
    }

    static Boolean placeUnit(int player, Unit unit, int row, int column) {
        if (unit.special == 0 && checkMatch(player, unit, row, column) == null) {
            return null;
        } else if (unit.height > 1) {
            unit.topRow = row;
        }
        for (int i = 0; i < unit.length; i++) {
            for (int j = 0; j < unit.height; j++) {
                board[player][row + j][column - i] = unit;
            }
        }
        return true;
    }

    static Integer checkMatch(int player, Unit unit, int row, int column) {
        int count = 0;
        if (column < 5 && board[player][row][column + 1].match(unit)) {
            if (board[player][row][column + 2].match(unit)) {
                if (reinforcing) {
                    return null;
                } else {
                    // become attack
                }
                count++;
            } else if (board[player][row][column + 2].special > 0
                    && unit.color.equals(board[player][row][column + 2].color)) {
                if (board[player][row][column + 2].height == 1) {
                    if (reinforcing) {
                        return null;
                    } else {
                        // become special attack
                    }
                    count++;
                } else if (checkSpecialMatch(player, board[player][row][column + 2], row, column + 2)) {
                    if (reinforcing) {
                        return null;
                    } else {
                        // become tall attack
                    }
                    count++;
                }
            }
        }

        int up = 0;
        int down = 0;
        if (row != 0 && board[player][row - 1][column].match(unit)) {
            up++;
            if (row != 1 && board[player][row - 2][column].match(unit)) {
                up++;
            }
        }
        if (row != 5 && board[player][row + 1][column].match(unit)) {
            down++;
            if (row != 4 && board[player][row + 2][column].match(unit)) {
                down++;
            }
        }
        if (up + down >= 2) {
            if (reinforcing) {
                return null;
            } else {
                // becomeDefence(player, row - up, up + down, column);
            }
            count++;
        }

        return count;
    }

    static boolean checkSpecialMatch(int player, Unit unit, int row, int column) {
        for (int i = 0; i < unit.height; i++) {
            if (unit.height + i != unit.topRow) {
                if (!(board[player][row][column - 1].match(unit) && board[player][row][column - 2].match(unit))) {
                    return false;
                }
            }
        }
        return true;
    }

    static void becomeAttack(int player, int row, int column) {
        board[player][row][column] = BLANK_UNIT;
        board[player][row + 1][column] = BLANK_UNIT;
        board[player][row + 2][column] = BLANK_UNIT;
    }

    static void becomeSpecialAttack(int player, Unit unit, int row, int column) {
        for (int i = 0; i < unit.length + 2; i++) {
            board[player][row + i][column] = BLANK_UNIT;
        }
    }

    static void becomeTallSpecialAttack(int player, Unit unit, int column) {
        for (int i = 0; i < unit.length + 2; i++) {
            for (int j = 0; j < unit.height; j++) {
                board[player][unit.topRow + i][column + j] = BLANK_UNIT;
            }
        }
    }

    static void becomeDefence(int player, int topRow, int rows, int column) throws CloneNotSupportedException {
        for (int i = 0; i < rows; i++) {
            board[player][topRow + i][column] = BLANK_UNIT;
        }
        for (int i = 0; i < rows; i++) {
            int temp = -1;
            for (int j = 6; j >= 0 && temp == -1; j--) {
                if (board[player][topRow + i][j].special == -1) 
                    temp = j;
            }
            if (temp == -1) {
                shiftUnitsLeft(player, topRow + i, column, 1);
                board[player][topRow + i][6] = (Unit) unitList.get(0).clone();
            } else {
                board[player][topRow + i][temp] = (Unit) unitList.get(1).clone();
                shiftUnitsRight(player, topRow + i, column);
            }
        }
    }

    static void shiftUnitsLeft(int player, int row, int column, int distance) {
        for (int i = column + 1; i < 7 && board[player][row][i].special >= 0; i++) {
            if (board[player][row][i].height == 1) {
                shiftUnitLeft(player, row, i, distance);
            } else
                System.err.println("not implemented");
        }
    }

    static void shiftUnitLeft(int player, int row, int column, int distance) {
        if (column - distance >= 0) {
            board[player][row][column - distance] = board[player][row][column];
        } else {
            if (board[player][row][column].length != 1 || board[player][row][column].height != 1) {
                System.err.println("not implemented");
            }
            playerPoints[player] += board[player][row][column].cost;
        }
        board[player][row][column] = BLANK_UNIT;
    }

    static void shiftUnitsRight(int player, int row, int column) {
        for (int i = column - 1; i >= 0 && board[player][row][i] != ; i--) {
            if (board[player][row][i].height == 1) {
                shiftUnitRight(player, row, i);
            } else
                System.err.println("not implemented");
        }
    }

    static void shiftUnitRight(int player, int row, int column) {
        boolean temp = true;
        for (int i = column + 1; i < 7 && temp; i++) {
            if (board[player][row][i] == BLANK_UNIT) {
                board[player][row][i] = board[player][row][i - 1];
                board[player][row][i - 1] = BLANK_UNIT;
            } else {
                temp = false;
            }
        }
    }

    static void removeUnit(int player, int row, int column) {
        if (board[player][row][column].height == 1) {
            if (board[player][row][column].length == 1) {
                playerPoints[player] += board[player][row][column].cost;
                board[player][row][column] = BLANK_UNIT;
                shiftUnitsRight(player, row, column);
            } else
                System.err.println("not implemented");
        } else
            System.err.println("not implemented");
    }
             */
}