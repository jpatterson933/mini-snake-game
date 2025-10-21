import {
    ctx,
    canvas,
    scoreDisplay,
    multiplierDisplay,
    speedIndicator,
    activePowerUpsDisplay,
    gameOverMessage
} from './domElements.js';
import { CELL_SIZE, TYPE_COLORS } from './constants.js';

let screenShakeOffsetX = 0;
let screenShakeOffsetY = 0;

export function renderGameStateOnCanvas(gameState, onGameOver) {
    applyVisualEffects(gameState.visualEffects);

    ctx.save();
    ctx.translate(screenShakeOffsetX, screenShakeOffsetY);

    clearCanvas();
    drawParticles(gameState.particles);
    drawSnakeTrail(gameState.trail, gameState.colors, gameState.activePowerUps);
    drawFood(gameState.food);

    if (gameState.powerUp) {
        drawPowerUp(gameState.powerUp);
    }

    ctx.restore();

    updateScoreDisplay(gameState.score, gameState.scoreMultiplier);
    updateSpeedIndicator(gameState.gameSpeed);
    updateActivePowerUpsDisplay(gameState.activePowerUps);

    if (gameState.isGameOver) {
        showGameOverMessage();
        onGameOver(gameState.score);
    }
}

function clearCanvas() {
    ctx.fillStyle = '#2a2a2a';
    ctx.fillRect(0, 0, canvas.width, canvas.height);
}

function drawSnakeTrail(segments, colors, activePowerUps) {
    const hasInvincibility = activePowerUps?.some(p => p.type === 'Invincibility');
    const hasGhostMode = activePowerUps?.some(p => p.type === 'GhostMode');

    segments.forEach((segment, index) => {
        const color = colors[Math.min(index, colors.length - 1)];

        if (hasGhostMode) {
            ctx.globalAlpha = 0.5;
            ctx.shadowBlur = 20;
            ctx.shadowColor = '#9370DB';
        }

        if (hasInvincibility && index === 0) {
            ctx.shadowBlur = 15;
            ctx.shadowColor = '#00BFFF';
        }

        ctx.fillStyle = color;
        ctx.fillRect(
            segment.x * CELL_SIZE,
            segment.y * CELL_SIZE,
            CELL_SIZE - 2,
            CELL_SIZE - 2
        );

        ctx.globalAlpha = 1.0;
        ctx.shadowBlur = 0;
    });
}

function drawFood(food) {
    ctx.save();
    ctx.shadowBlur = 10;
    ctx.shadowColor = food.color;
    ctx.fillStyle = food.color;
    ctx.beginPath();
    ctx.arc(
        food.position.x * CELL_SIZE + CELL_SIZE / 2,
        food.position.y * CELL_SIZE + CELL_SIZE / 2,
        CELL_SIZE / 2 - 2,
        0,
        Math.PI * 2
    );
    ctx.fill();
    ctx.restore();
}

function drawPowerUp(powerUp) {
    const centerX = powerUp.position.x * CELL_SIZE + CELL_SIZE / 2;
    const centerY = powerUp.position.y * CELL_SIZE + CELL_SIZE / 2;

    ctx.save();
    ctx.translate(centerX, centerY);

    ctx.shadowBlur = 20 * powerUp.pulse;
    ctx.shadowColor = powerUp.color;

    ctx.fillStyle = powerUp.color;
    ctx.beginPath();
    ctx.arc(0, 0, (CELL_SIZE / 2) * powerUp.pulse, 0, Math.PI * 2);
    ctx.fill();

    ctx.font = `${CELL_SIZE}px Arial`;
    ctx.textAlign = 'center';
    ctx.textBaseline = 'middle';
    ctx.fillText(powerUp.icon, 0, 0);

    ctx.restore();
}

function drawParticles(particles) {
    if (!particles) return;

    particles.forEach(particle => {
        ctx.save();
        ctx.globalAlpha = particle.opacity;
        ctx.fillStyle = particle.color;
        ctx.beginPath();
        ctx.arc(
            particle.x * CELL_SIZE,
            particle.y * CELL_SIZE,
            particle.size * CELL_SIZE,
            0,
            Math.PI * 2
        );
        ctx.fill();
        ctx.restore();
    });
}

function updateScoreDisplay(score, scoreMultiplier) {
    scoreDisplay.textContent = score;
    multiplierDisplay.textContent = scoreMultiplier > 1 ? `x${scoreMultiplier}` : '';
}

function updateSpeedIndicator(gameSpeed) {
    if (gameSpeed > 1) {
        speedIndicator.textContent = '⚡ SPEED BOOST';
    } else if (gameSpeed < 1) {
        speedIndicator.textContent = '⏰ SLOW MOTION';
    } else {
        speedIndicator.textContent = '';
    }
}

function updateActivePowerUpsDisplay(activePowerUps) {
    if (!activePowerUps || activePowerUps.length === 0) {
        activePowerUpsDisplay.innerHTML = '';
        return;
    }

    activePowerUpsDisplay.innerHTML = activePowerUps.map(powerUp => {
        const progressPercent = (powerUp.progress * 100).toFixed(0);
        const color = TYPE_COLORS[powerUp.type] || '#fff';
        return `
            <div style="display: inline-block; margin-right: 15px; padding: 5px 10px; background: rgba(0,0,0,0.5); border-radius: 5px; border: 2px solid ${color};">
                <span style="color: ${color};">${powerUp.type}</span>
                <div style="width: 100px; height: 4px; background: #333; margin-top: 3px; border-radius: 2px;">
                    <div style="width: ${progressPercent}%; height: 100%; background: ${color}; border-radius: 2px;"></div>
                </div>
            </div>
        `;
    }).join('');
}

function applyVisualEffects(visualEffects) {
    if (!visualEffects) return;

    screenShakeOffsetX = 0;
    screenShakeOffsetY = 0;

    visualEffects.forEach(effect => {
        if (effect.type === 'ScreenShake') {
            screenShakeOffsetX = (Math.random() - 0.5) * effect.intensity;
            screenShakeOffsetY = (Math.random() - 0.5) * effect.intensity;
        }
    });
}

function showGameOverMessage() {
    gameOverMessage.style.display = 'block';
}
