import axios from 'axios';
import { Superinvestor } from '../components';

const url = `http://localhost:5195/v1/superinvestors`;

export const list = async (skip: number, take: number) => {
    try {
        const response = await axios.get<Superinvestor[]>(`${url}?skip=${skip}&take=${take}`);
        return response.data;
    } catch (error) {
        console.error('Error fetching data:', error);
        throw error;
    }
};