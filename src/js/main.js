import {
    startButton,
    exitButton,
    instructionsButton,
    closeInstructionsButton,
    saveHighScoreButton,
    skipHighScoreButton
} from './domElements.js';
import {
    showGameScreenAndStartGame,
    showMainMenu,
    showInstructions,
    closeInstructions
} from './navigation.js';
import { saveHighScore, closeHighScoreModal } from './highScore.js';

function initializeEventListeners() {
    startButton.addEventListener('click', showGameScreenAndStartGame);
    exitButton.addEventListener('click', showMainMenu);
    instructionsButton.addEventListener('click', showInstructions);
    closeInstructionsButton.addEventListener('click', closeInstructions);
    saveHighScoreButton.addEventListener('click', saveHighScore);
    skipHighScoreButton.addEventListener('click', closeHighScoreModal);
}

initializeEventListeners();
