import java.util.ArrayList;
import java.util.Random;

public class Board {

    static Random rand = new Random();

    int player;
    int width;
    int height;
    Unit[][] board;

    int unitsAdded = 0;

    public Board(int player, int width, int height) {
        this.player = player;
        this.width = width;
        this.height = height;
        board = new Unit[height][width];
        clear();
    }

    void clear() {
        unitsAdded = 0;
        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                board[y][x] = Unit.BLANK_UNIT;
            }
        }
    }

    void print() { // debug only
        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                System.out.print(board[y][x].color + board[y][x].icon + Unit.ANSI_RESET);
            }
            System.out.println();
        }
        System.out.println();
    }

    boolean addUnit(Unit unit) { // places a unit in a random row
        ArrayList<Integer> open = new ArrayList<Integer>();
        ArrayList<Integer> spots = new ArrayList<Integer>();
        for (int i = 0; i < 7 - unit.height; i++) {
            int spot = findSpot(unit, i, unit.length - 1);
            if (spot != -1) {
                if (!Main.reinforcing || checkMatch(unit, i, spot) == 0) {
                    open.add(i);
                    spots.add(spot);
                }
            }
        }
        if (open.size() == 0) {
            System.out.println("no open spots");
            return false;
        }
        int tempRand = rand.nextInt(open.size());
        unit.topRight = new int[] { open.get(tempRand), spots.get(tempRand) };
        for (int y = 0; y < unit.height; y++) {
            for (int x = 0; x < unit.length; x++) {
                board[unit.topRight[0] + y][unit.topRight[1] + x] = unit;
            }
        }
        unitsAdded++;
        return true;
    }

    int checkFill(int column) {
        int num = 0;
        for (int i = 0; i < height; i++) {
            if (board[i][column] != Unit.BLANK_UNIT)
                num++;
        }
        System.out.println(num);
        return num;
    }

    int findSpot(Unit unit, int row, int column) {
        int[] spot = new int[unit.height];
        for (int y = 0; y < unit.height; y++) {
            spot[y] = -1;
            for (int i = column; i < width && board[y + row][i] == Unit.BLANK_UNIT; i++) {
                spot[y] = i;
            }
            if (spot[y] == -1) {
                return -1;
            }
        }
        int temp = spot[0];
        for (int y = 1; y < unit.height; y++) {
            if (temp > spot[y])
                temp = spot[y];
        }
        return temp - (unit.length - 1);
    }

    int checkMatch(Unit unit, int row, int column) {
        if (unit.special != 0)
            return 0;
        int matches = 0;
        if (column < 5 && board[row][column + 1].match(unit)) {
            if (board[row][column + 2].match(unit)) {
                if (Main.reinforcing) {
                    return -1;
                } else {
                    // become attack
                }
                matches++;
            } else if (board[row][column + 2].special > 0
                    && unit.color.equals(board[row][column + 2].color)) {
                if (board[row][column + 2].height == 1) {
                    if (Main.reinforcing) {
                        return -1;
                    } else {
                        // become special attack
                    }
                    matches++;
                } else if (checkSpecialMatch(board[row][column + 2], row, column + 2)) {
                    if (Main.reinforcing) {
                        return -1;
                    } else {
                        // become tall attack
                    }
                    matches++;
                }
            }
        }

        int up = 0;
        int down = 0;
        if (row != 0 && board[row - 1][column].match(unit)) {
            up++;
            if (row != 1 && board[row - 2][column].match(unit)) {
                up++;
            }
        }
        if (row != 5 && board[row + 1][column].match(unit)) {
            down++;
            if (row != 4 && board[row + 2][column].match(unit)) {
                down++;
            }
        }

        if (up + down >= 2) {
            if (Main.reinforcing) {
                return -1;
            } else {
                // becomeDefence(row - up, up + down, column);
            }
            matches++;
        }
        return matches;
    }

    boolean checkSpecialMatch(Unit unit, int row, int column) {
        for (int i = 0; i < unit.height; i++) {
            if (unit.height + i != unit.topRight[0]) {
                if (!(board[row][column - 1].match(unit) && board[row][column - 2].match(unit))) {
                    return false;
                }
            }
        }
        return true;
    }

    void removeUnit(int row, int column) {
        removeUnit(board[row][column], row, column);
    }

    void removeUnit(Unit unit, int row, int column) {
        if (unit == Unit.BLANK_UNIT)
            return;
        Main.playerPoints[player] += unit.cost;
        for (int y = 0; y < unit.height; y++) {
            for (int x = 0; x < unit.length; x++) {
                board[unit.topRight[0] + y][unit.topRight[1] + x] = Unit.BLANK_UNIT;
            }
        }
        shiftUnitsRight(unit.topRight[0], unit.topRight[1] - 1);
    }

    void shiftUnitsRight(int row, int column) {
        if (column < 0)
            return;
        Unit unit = board[row][column];
        if (unit == Unit.BLANK_UNIT)
            return;
        for (int y = 0; y < unit.height; y++) {
            for (int x = 0; x < unit.length; x++) {
                board[unit.topRight[0] + y][unit.topRight[1] + x] = Unit.BLANK_UNIT;
            }
        }
        int temp = findSpot(unit, unit.topRight[0], unit.topRight[1]);
        if (temp == -1) {
            System.err.println("panec");
        }
        int temp2 = unit.topRight[1];
        unit.topRight[1] = temp;
        for (int y = 0; y < unit.height; y++) {
            for (int x = 0; x < unit.length; x++) {
                board[unit.topRight[0] + y][unit.topRight[1] + x] = unit;
            }
        }
        for (int y = 0; y < unit.height; y++) {
            shiftUnitsRight(unit.topRight[0] + y, temp2 - 1);
        }
    }
}
