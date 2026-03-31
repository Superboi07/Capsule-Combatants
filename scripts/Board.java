import java.util.ArrayList;
import java.util.Random;

public class Board {

    // I realized speical units cant be more than 2 tall
    // I can't be bothered with optimizing it rn

    // they can still be more than 2 long tho

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

    boolean addUnit(Unit unit) throws CloneNotSupportedException { // places a unit in a random row
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

    int checkMatch(Unit unit, int row, int column) throws CloneNotSupportedException {
        if (unit.special != 0 || unit == Unit.BLANK_UNIT)
            return 0;
        boolean attacking = false;
        boolean defending = false;
        int matches = 0;
        if (column < 5 && board[row][column + 1].match(unit)) {
            if (board[row][column + 2].match(unit)) {
                if (Main.reinforcing) {
                    return -1;
                } else {
                    attacking = true;
                }
                matches++;
            }
            if (board[row][column + 2].special > 0
                    && unit.color.equals(board[row][column + 2].color)) {
                if (board[row][column + 2].height == 1) {
                    if (Main.reinforcing) {
                        return -1;
                    } else {
                        attacking = true;
                    }
                    matches++;
                } else if (checkSpecialMatch(board[row][column + 2], row, column + 2)) {
                    if (Main.reinforcing) {
                        return -1;
                    } else {
                        attacking = true;
                    }
                    matches++;
                }
            }
        }

        boolean temp = true;
        int up = 0;
        int down = 0;
        for (int i = 1; temp && row - i > -1; i++) {
            if (board[row - i][column].match(unit)) {
                up++;
            } else {
                temp = false;
            }
        }
        temp = true;
        for (int i = 1; temp && row + i < 6; i++) {
            if (board[row + i][column].match(unit)) {
                down++;
            } else {
                temp = false;
            }
        }

        if (up + down > 1) {
            if (Main.reinforcing) {
                return -1;
            } else {
                defending = true;
            }
            matches++;
        }
        if (attacking) {
            if (board[row][column + 2].special > 0) {
                if (board[row][column + 2].height > 1) {
                    if (board[row][column + 2].topRight[0] != row) {
                        if (up != 0) {
                            up--;
                        }
                    } else {
                        if (down != 0) {
                            down--;
                        }
                    }
                }
                becomeSpecialAttack(board[row][column + 2]);
            } else {
                becomeAttack(row, column + 2);
            }
        }
        if (defending) {
            for (int i = 1; i <= up; i++) {
                becomeDefence(row - i, column);
            }
            for (int i = 1; i <= down; i++) {
                becomeDefence(row + i, column);
            }
            if (!attacking) {
                becomeDefence(row, column);
            }
        }
        if (matches > 0) {
            for (int i = -up; i <= down; i++) {
                matches += matchRow(row + i);
            }
        }
        return matches;
    }

    int matchRow(int row) throws CloneNotSupportedException {
        int matches = 0;
        for (int i = 0; i < 7; i++) {
            matches += checkMatch(board[row][i], row, i);
        }
        return matches;
    }

    boolean checkSpecialMatch(Unit unit, int row, int column) {
        for (int i = 0; i < unit.height; i++) {
            if (unit.height + i != row) {
                if (!(board[row][column - 1].match(unit) && board[row][column - 2].match(unit))) {
                    return false;
                }
            }
        }
        return true;
    }

    void becomeAttack(int row, int column) {
        Unit unit = board[row][column];
        unit.color += Unit.ANSI_ITALICS;
        unit.health = unit.attack;
        unit.isAttacking = true;
        int temp = 6;
        for (int i = column + 1; i < 7; i++) {
            if (board[row][i].special < 0) {
                temp = i - 1;
            }
        }
        for (int i = 0; i < 3; i++) {
            board[row][column - i] = Unit.BLANK_UNIT;
            shiftUnitsLeft(row, temp - i);
            board[row][temp - i] = unit;
        }
        unit.topRight[1] = temp;
    }

    void becomeSpecialAttack(Unit unit) {
        unit.color += Unit.ANSI_ITALICS;
        unit.health = unit.attack;
        unit.isAttacking = true;
        for (int y = 0; y < unit.height; y++) {
            for (int i = 0; i < 2 + unit.length; i++) {
                board[unit.topRight[0] + y][unit.topRight[1] - i] = Unit.BLANK_UNIT;
            }
        }
        int temp = 7 - unit.length;
        for (int i = unit.topRight[1] + 1; i < 7; i++) {
            if (board[unit.topRight[0]][i].special < 0) {
                temp = i - 1;
            }
        }
        if (unit.height == 2) {
            int temp1 = 7 - unit.length;
            for (int i = unit.topRight[1] + 1; i < 7; i++) {
                if (board[unit.topRight[0] + 1][i].special < 0) {
                    temp1 = i - 1;
                }
            }
            temp = Math.min(temp, temp1);
        }
        for (int y = 0; y < unit.height; y++) {
            for (int x = 0; x < unit.length; x++) {
                shiftUnitsLeft(unit.topRight[0] + y, temp - x);
                board[unit.topRight[0] + y][temp - x] = unit;
            }
        }
        unit.topRight[1] = temp;
        for (int y = 0; y < unit.height; y++) { // regression, sometimes
            shiftUnitsRight(unit.topRight[0] + y, unit.topRight[1] - (1 + unit.length));
        }
        System.out.println(unit.length); // debug message for 2 long special unit enidng up 3 long
    }

    void becomeDefence(int row, int column) throws CloneNotSupportedException {
        // logic broke, resulting in making teir 2 defences despite not having a vaid combo
        board[row][column] = Unit.BLANK_UNIT;
        int temp = -1;
        for (int i = 6; i > column; i--) {
            if (board[row][i].special == -1) {
                temp = i;
            }
        }
        if (temp == -1) {
            temp = 6;
            for (int i = column + 1; i < 7; i++) {
                if (board[row][i].special == -2) {
                    temp = i - 1;
                }
            }
            shiftUnitsLeft(row, temp);
            board[row][temp] = (Unit) Main.unitList.get(0).clone();
        } else {
            board[row][temp] = (Unit) Main.unitList.get(1).clone();
            shiftUnitsRight(row, column - 1);
        }
    }

    int removeUnit(int row, int column) throws CloneNotSupportedException {
        Unit unit = board[row][column];
        removeUnit(unit, row, column);
        int temp = -1;
        for (int i = 0; i < unit.height; i++) {
            temp += matchRow(unit.topRight[0] + i);
        }
        return temp;
    }

    void removeUnit(Unit unit, int row, int column) {
        if (unit == Unit.BLANK_UNIT)
            return;
        Main.playerPoints[player] += unit.cost;
        for (int y = 0; y < unit.height; y++) {
            for (int x = 0; x < unit.length; x++) {
                board[unit.topRight[0] + y][unit.topRight[1] + x] = Unit.BLANK_UNIT;
            }
            shiftUnitsRight(unit.topRight[0] + y, unit.topRight[1] - 1);
        }
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

    void shiftUnitsLeft(int row, int column) {
        Unit unit = board[row][column];
        if (unit == Unit.BLANK_UNIT) {
            return;
        }
        for (int y = 0; y < unit.height; y++) {
            for (int x = 0; x < unit.length; x++) {
                board[unit.topRight[0] + y][unit.topRight[1] + x] = Unit.BLANK_UNIT;
            }
        }
        if (unit.topRight[1] - 1 < 0) {
            Main.playerPoints[player] += unit.cost;
            return;
        }
        for (int y = 0; y < unit.height; y++) {
            shiftUnitsLeft(unit.topRight[0] + y, unit.topRight[1] - 1);
        }
        unit.topRight[1] -= 1;
        for (int y = 0; y < unit.height; y++) {
            for (int x = 0; x < unit.length; x++) {
                board[unit.topRight[0] + y][unit.topRight[1] + x] = unit;
            }
        }

    }
}
