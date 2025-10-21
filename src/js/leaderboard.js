import { leaderboardEntries } from './domElements.js';
import { RANK_MEDALS } from './constants.js';

export async function loadAndDisplayLeaderboard(connection) {
    try {
        const topFiveScores = await connection.invoke("GetTopFiveScores");
        renderLeaderboardEntries(topFiveScores);
    } catch (error) {
        console.error('Error loading leaderboard:', error);
        showLeaderboardError();
    }
}

function renderLeaderboardEntries(scores) {
    if (!scores || scores.length === 0) {
        showEmptyLeaderboard();
        return;
    }

    const entriesHtml = scores.map((score, index) =>
        createLeaderboardEntryHtml(score, index)
    ).join('');

    leaderboardEntries.innerHTML = entriesHtml;
}

function createLeaderboardEntryHtml(score, index) {
    const rankMedal = RANK_MEDALS[index];
    const playerName = score.playerName;
    const playerScore = score.score;

    return `
        <div class="leaderboard-entry">
            <span class="leaderboard-rank">${rankMedal}</span>
            <span class="leaderboard-player">${playerName}</span>
            <span class="leaderboard-score">${playerScore}</span>
        </div>
    `;
}

function showEmptyLeaderboard() {
    leaderboardEntries.innerHTML = '<div class="leaderboard-empty">No scores yet!</div>';
}

function showLeaderboardError() {
    leaderboardEntries.innerHTML = '<div class="leaderboard-empty" style="color: #ff4444;">Failed to load</div>';
}
