import { useEffect, useState } from "react";
import Counter from "~/components/Counter/Counter";

export default function TotalStorageCount() {
    const [totalStorageUsed, setTotalStorageUsed] = useState<number>(0);
    useEffect(() => {
        const fetchData = async () => {
            fetch("http://localhost:5175/api/admin/storage/total-used", {
                method: "GET",
                credentials: "include",
            }).then(response => response.json()).then(data => {setTotalStorageUsed(data.totalStorageUsed);});
        };
        fetchData();
        const interval = setInterval(() => {
            fetchData();
        }, 5000);

        return () => clearInterval(interval);   
    }, []);
    
    if (!totalStorageUsed) {
        return <Counter title="Total Storage Used (MB)" count={0} />;
    }
    return <Counter title="Total Storage Used (MB)" count={totalStorageUsed} />;
}