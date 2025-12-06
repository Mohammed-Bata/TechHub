// JavaScript source code
import http from 'k6/http';
import { sleep } from 'k6';

const BASE_URL = "http://localhost:5076";

export const options = {
    vus: 100,            // 100 users
    duration: '30s'     // for 30 seconds
};

export default function () {
    http.get(`${BASE_URL}/api/Products/search?pageSize=9&pageNumber=1`);
    sleep(1);
}
