package com.github.superboi07.capsulecombatants;

import java.io.FileReader;
//import org.json.simple.*;
import org.json.simple.JSONObject;
//import org.json.simple.parser.*;
class Main {

    static String[][][] board = new String[2][6][7];

    static int startingPoints = 35;
    static int[] points = new int[] {startingPoints, startingPoints};

    //static Unit[][] units = new Unit[2][5];
    
    public static void main(String[] args) {
        //clearBoard();
        //printBoard();

        //reinforce(0);
        //reinforce(1);

        // The file JSON.json is parsed
        //Object object = new JSONParser().parse(new FileReader("unitList.json"));

        // objc is convereted to JSON object
        //JSONObject jsonObject = (JSONObject)object;

        //String bame = (String)jsonObject.get("name");
        //System.out.println(bame);

        //JSONObject geekWriterObject = new JSONObject();
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