import * as signalR from "@microsoft/signalr";
import "./css/main.css";
import { Player } from './player';
import { GameState } from "./gameState";
import { CanvasHelper } from "./canvas-helper";
import { InteractionHelper } from "./interaction-helper";
import { Point } from "./point";

const divMessages: HTMLDivElement = document.querySelector("#divMessages");
const tbMessage: HTMLInputElement = document.querySelector("#tbMessage");
const btnSend: HTMLButtonElement = document.querySelector("#btnSend");
const username = new Date().getTime();

const canvas: HTMLCanvasElement = document.querySelector('#myCanvas');
const canvasHelper = new CanvasHelper(canvas);
const interactionHelper = new InteractionHelper(canvas);

var players: Player[] = [];
var me: Player = new Player({x: 50, y: 50});
var lastMouseDirectionVectorPoint = new Point({x: 0, y: 0});

const gameFrame = () => {
    canvasHelper.clearCanvas();
    if (me) {
      canvasHelper.drawBackground(me);
      players.forEach(player => canvasHelper.drawPlayer(me, player));
    }
}

const sendUserInteractions = () => {
  const mouseDirectionVectorPoint = interactionHelper.findMouseDirectionVectorPoint();
  if (!lastMouseDirectionVectorPoint.equals(mouseDirectionVectorPoint)) {
    lastMouseDirectionVectorPoint = mouseDirectionVectorPoint;
    connection.send("mouseMoved", mouseDirectionVectorPoint);
  }
}

const animationFrame = () => {
    try {
        gameFrame();
        sendUserInteractions();
    } catch (e) {
        console.log(e);
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