import * as signalR from "@microsoft/signalr";
import "./css/main.css";
import { Player } from './player';
import { GameState } from "./gameState";
import { CanvasHelper } from "./canvas-helper";

const divMessages: HTMLDivElement = document.querySelector("#divMessages");
const tbMessage: HTMLInputElement = document.querySelector("#tbMessage");
const btnSend: HTMLButtonElement = document.querySelector("#btnSend");
const username = new Date().getTime();

const canvas: HTMLCanvasElement = document.querySelector('#myCanvas');
const canvasHelper = new CanvasHelper(canvas);

let players: Player[] = [];
let me: Player = new Player({x: 50, y: 50});

const gameFrame = () => {
    canvasHelper.clearCanvas();
    canvasHelper.drawBackground(me);
    players.forEach(player => canvasHelper.drawPlayer(me, player));
}

const animationFrame = () => {
    try {
        gameFrame();
    } catch (e) {
        console.log(e.message);
    }

    window.requestAnimationFrame(animationFrame);
}

window.requestAnimationFrame(animationFrame);

const connection = new signalR.HubConnectionBuilder()
    .withUrl("/hub")
    .build();

connection.on("messageReceived", (username: string, message: string) => {
  const m = document.createElement("div");

  m.innerHTML = `<div class="message-author">${username}</div><div>${message}</div>`;

  divMessages.appendChild(m);
  divMessages.scrollTop = divMessages.scrollHeight;
});

connection.on("gameStateUpdate", (gameState: GameState) => {
    players = gameState.players;
    me = gameState.players.find(p => p.id === connection.connectionId);
  });

connection.start().catch((err) => document.write(err));

tbMessage.addEventListener("keyup", (e: KeyboardEvent) => {
  if (e.key === "Enter") {
    send();
  }
});

btnSend.addEventListener("click", send);

function send() {
  connection.send("newMessage", username, tbMessage.value)
    .then(() => (tbMessage.value = ""));
}