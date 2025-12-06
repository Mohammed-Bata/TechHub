// JavaScript source code
import http from "k6/http";

const BASE_URL = "http://localhost:5076";

export const options = {
    stages: [
        { duration: '2s', target: 1 },
        { duration: '1s', target: 300 }, // spike
        { duration: '10s', target: 1 },
    ],
};

export default function () {
    http.get(`${BASE_URL}/api/Products/search?pageSize=9&pageNumber=1`);
}
