// JavaScript source code
import http from 'k6/http';
import { sleep } from 'k6';

const BASE_URL = "http://localhost:5076";

export const options = {
    stages: [
        { duration: '20s', target: 50 },   // 50 users
        { duration: '20s', target: 150 },  // 150 users
        { duration: '20s', target: 300 },  // 300 users
        { duration: '20s', target: 0 },    // back to 0
    ]
};

export default function () {
    http.get(`${BASE_URL}/api/Products/search?pageSize=9&pageNumber=1`);
    sleep(1);
}
