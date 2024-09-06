import React, { useEffect, useState } from 'react';
import { list } from '../services'; 
import { SuperinvestorPanel, Superinvestor } from '../components';

const Home: React.FC = () => {
    const [data, setData] = useState<Superinvestor[]>([]);
    const [loading, setLoading] = useState<boolean>(true);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        const getData = async () => {
            try {
                const result = await list(0, 3);
                setData(result);
                setLoading(false);
            } catch (error) {
                setError('Failed to fetch data');
                setLoading(false);
            }
        };

        getData(); 
    }, []);

    if (loading) {
        return <div>Loading...</div>;
    }

    if (error) {
        return <div>Error: {error}</div>;
    }

    return (
        <div>
            <SuperinvestorPanel data={data} />
        </div>
    );
};

export default Home;