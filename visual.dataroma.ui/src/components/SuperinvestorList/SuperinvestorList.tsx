import { useState, useEffect, useRef } from 'react';
import { list } from '../../services'; 
import { SuperinvestorPanel, Superinvestor } from '..';

const PAGE_SIZE = 5;
let currentPage = 0;

const SuperinvestorList = () => {
    const [data, setData] = useState<Superinvestor[]>([]);
    const [loading, setLoading] = useState<boolean>(true);
    const [error, setError] = useState<string | null>(null);
    const [hasMore, setHasMore] = useState<boolean>(true);
    const isInitialLoad = useRef(true);

    const getData = async () => {
        try {
            const result = await list(currentPage, PAGE_SIZE);
            if (result.length < PAGE_SIZE) {
                setHasMore(false); 
            }
            setData(prevData => [...prevData, ...result]);
            
            currentPage += PAGE_SIZE;

            setLoading(false);
        } catch (error) {
            setError('Failed to fetch data');
            setLoading(false);
        }
    };

    useEffect(() => {
        if (isInitialLoad.current) {
            getData();
            isInitialLoad.current = false;
        }
    });

    useEffect(() => {
        const handleScroll = () => {
            if (hasMore && window.innerHeight + document.documentElement.scrollTop > document.documentElement.offsetHeight) {
                if (!loading) { 
                    setLoading(true);
                    getData(); 
                }
            }
        };

        window.addEventListener('scroll', handleScroll);
        return () => window.removeEventListener('scroll', handleScroll);
    }, [loading, hasMore]);

    if (loading && data.length === 0) {
        return <div>Loading...</div>;
    }

    if (error) {
        return <div>Error: {error}</div>;
    }

    return (
        <div>
            <SuperinvestorPanel data={data} />
            {loading && <div>Loading more...</div>}
        </div>
    );
};

export default SuperinvestorList;
