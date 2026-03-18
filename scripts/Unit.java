class Unit implements Cloneable {
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
    int topRow = -1;

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