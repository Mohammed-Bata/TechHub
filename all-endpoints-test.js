// JavaScript source code
import http from "k6/http";
import { Trend } from "k6/metrics";

const BASE_URL = "https://localhost:7040";

let ttfb = new Trend("ttfb");

const endpoints = [
    "/api/Products",
    "/api/Categories",
    "/api/Users",
    "/api/Orders",
]; // add all your endpoints here

export default function () {
    for (const ep of endpoints) {
        let res = http.get(`${BASE_URL}${ep}`);
        ttfb.add(res.timings.waiting);
    }
}
