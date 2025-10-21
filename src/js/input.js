import { DIRECTION_MAP } from './constants.js';

let activeConnection = null;

export function listenForPlayerKeyboardInput(connection) {
    activeConnection = connection;
    document.addEventListener('keydown', handleKeyPress);
}

function handleKeyPress(event) {
    const direction = DIRECTION_MAP[event.key];
    if (direction && activeConnection) {
        activeConnection.invoke("ChangeDirection", direction);
        event.preventDefault();
    }
}
