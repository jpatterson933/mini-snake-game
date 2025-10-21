import * as signalR from '@microsoft/signalr';
import { renderGameStateOnCanvas } from './renderer.js';
import { loadAndDisplayLeaderboard } from './leaderboard.js';
import { listenForPlayerKeyboardInput } from './input.js';
import { setGameConnection, checkAndShowHighScoreModal } from './highScore.js';

let connection = null;

export function connectToGameServerAndStart() {
    connection = new signalR.HubConnectionBuilder()
        .withUrl("/gameHub")
        .build();

    connection.on("UpdateGameState", (gameState) => {
        renderGameStateOnCanvas(gameState, checkAndShowHighScoreModal);
    });

    connection.start()
        .then(() => {
            setGameConnection(connection);
            connection.invoke("StartNewGame");
            loadAndDisplayLeaderboard(connection);
        })
        .catch(err => console.error(err));

    listenForPlayerKeyboardInput(connection);
}

export function disconnectFromGameServer() {
    if (connection) {
        connection.stop();
    }
}
