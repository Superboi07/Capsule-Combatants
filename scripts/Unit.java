class Unit implements Cloneable {
    public static final String ANSI_RESET = "\u001B[0m";
    public static final String ANSI_BLACK = "\u001B[30m";
    public static final String ANSI_RED = "\u001B[31m";
    public static final String ANSI_GREEN = "\u001B[32m";
    public static final String ANSI_YELLOW = "\u001B[33m";
    public static final String ANSI_BLUE = "\u001B[34m";
    public static final String ANSI_PURPLE = "\u001B[35m";
    public static final String ANSI_CYAN = "\u001B[36m";
    public static final String ANSI_WHITE = "\u001B[37m";
    public static final String[][] COLORS = new String[][] { { "red", ANSI_RED }, { "blue", ANSI_BLUE },
            { "green", ANSI_GREEN } };
    static final Unit BLANK_UNIT = new Unit();
    
    String name;
    char icon;
    int health;
    int attack;
    int growth;
    int speed;
    int length;
    int height;
    int cost;
    int special;
    String color = null;
    int max = -1;
    int[] topRight = new int[] {-1,-1};

    public Unit(String name, char icon, int health, int attack, int growth, int speed, int length, int height, int cost,
            int special) {
        this.name = name;
        this.icon = icon;
        this.health = health;
        this.attack = attack;
        this.growth = growth;
        this.speed = speed;
        this.length = length;
        this.height = height;
        this.cost = cost;
        this.special = special;
        if (special == 1) {
            max = 2;
        } else if (special == 2) {
            max = 1;
        }
    }

    public Unit() {
        this.name = null;
        this.icon = '_';
        this.health = 0;
        this.attack = 0;
        this.growth = 0;
        this.speed = 0;
        this.length = 1;
        this.height = 1;
        this.cost = 0;
        this.special = 0;
        this.color = "\u001B[30m";
    }

    public boolean match(Unit unit) {
        if (special == 0 && color == unit.color) {
            return true;
        } else {
            return false;
        }
    }

    @Override
    public Object clone() throws CloneNotSupportedException {
        return super.clone();
    }
}