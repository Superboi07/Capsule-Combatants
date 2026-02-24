class Main {

    static String[][][] board = new String[2][6][7];

    static int startingPoints = 35;
    static int[] points = new int[] {startingPoints, startingPoints};

    static Unit[][] units = new Unit[2][5];
    
    public static void main(String[] args) {
        clearBoard();
        printBoard();

        reinforce(0);
        reinforce(1);    
    }

    static void clearBoard() {
        for (int n = 0; n < 2; n++) {
            for (int i = 0; i < 6; i++) {
                for (int j = 0; j < 7; j++) {
                    board[n][i][j] = "_";
                }
            }
        }
    }

    static void printBoard() {
        for (int y = 0; y < 6; y++) {
            for (int x = 0; x < 7; x++) {
                System.out.print(board[0][y][x]);
            }
            System.out.print("|");
            for (int x = 6; x > -1; x--) {
                System.out.print(board[1][y][x]);
            }
            System.out.println();
        }
    }

    static void reinforce(int player) {

    }
}