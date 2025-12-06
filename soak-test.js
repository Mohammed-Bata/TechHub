// JavaScript source code
import http from "k6/http";
import { sleep } from "k6";

const BASE_URL = "https://localhost:7040";

export const options = {
    vus: 20,
    duration: "10m", // 10 minutes
};

export default function () {
    http.get(`${BASE_URL}/api/Products`);
    sleep(1);
}
