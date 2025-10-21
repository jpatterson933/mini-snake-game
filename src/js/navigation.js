import {
    mainMenu,
    gameScreen,
    instructionsModal,
    gameOverMessage
} from './domElements.js';
import { connectToGameServerAndStart, disconnectFromGameServer } from './connection.js';

export function showGameScreenAndStartGame() {
    mainMenu.classList.remove('active');
    gameScreen.classList.add('active');
    gameOverMessage.style.display = 'none';
    connectToGameServerAndStart();
}

export function showMainMenu() {
    disconnectFromGameServer();
    gameScreen.classList.remove('active');
    mainMenu.classList.add('active');
}

export function showInstructions() {
    instructionsModal.classList.add('active');
}

export function closeInstructions() {
    instructionsModal.classList.remove('active');
}
