import {
    highScoreModal,
    finalScore,
    playerNameInput,
    errorMessage,
    saveHighScoreButton
} from './domElements.js';
import { loadAndDisplayLeaderboard } from './leaderboard.js';

let currentGameScore = 0;
let gameConnection = null;

export function setGameConnection(connection) {
    gameConnection = connection;
}

export async function checkAndShowHighScoreModal(score) {
    try {
        currentGameScore = score;
        const isTopFive = await gameConnection.invoke("IsTopFiveScore", score);

        if (isTopFive) {
            finalScore.textContent = score;
            playerNameInput.value = '';
            errorMessage.style.display = 'none';
            highScoreModal.classList.add('active');
        }
    } catch (err) {
        console.error('Error checking high score:', err);
    }
}

export async function saveHighScore() {
    const playerName = playerNameInput.value.trim();

    if (!playerName) {
        displayValidationError('Please enter your name');
        return;
    }

    try {
        disableSaveButton();
        const result = await gameConnection.invoke("SaveHighScore", playerName, currentGameScore);

        if (result.success) {
            closeHighScoreModal();
            await loadAndDisplayLeaderboard(gameConnection);
        } else {
            displayValidationError(result.error);
        }
    } catch (error) {
        displayValidationError('Failed to save high score. Please try again.');
        console.error('Error saving high score:', error);
    } finally {
        enableSaveButton();
    }
}

function disableSaveButton() {
    saveHighScoreButton.disabled = true;
    saveHighScoreButton.textContent = 'Saving...';
}

function enableSaveButton() {
    saveHighScoreButton.disabled = false;
    saveHighScoreButton.textContent = 'Save High Score';
}

function displayValidationError(message) {
    errorMessage.textContent = message;
    errorMessage.style.display = 'block';
}

export function closeHighScoreModal() {
    highScoreModal.classList.remove('active');
}
