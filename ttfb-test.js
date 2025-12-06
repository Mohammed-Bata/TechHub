// JavaScript source code

import http from 'k6/http';
import { Trend } from 'k6/metrics';

const BASE_URL = "http://localhost:5076";

const endpoints = [
    { method: 'GET', url: '/api/Products/search?pageSize=9&pageNumber=1' },
    { method: 'GET', url: '/api/Products/3' },
    { method: 'POST', url: '/api/Users/login', body: { Email: 'admin@gmail.com', Password: '000000-Aa' } },

];

let ttfb = new Trend('ttfb');

export default function () {

    endpoints.forEach(ep => {
        let res;

        if (ep.method === 'GET') {
            res = http.get(`${BASE_URL}${ep.url}`);
        } else if (ep.method === 'POST') {
            res = http.post(`${BASE_URL}${ep.url}`, JSON.stringify(ep.body), {
                headers: { 'Content-Type': 'application/json' }
            });
        }

        // TTFB (Time to First Byte)
        ttfb.add(res.timings.waiting);
    });
}
