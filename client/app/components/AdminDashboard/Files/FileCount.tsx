import { useEffect, useState } from "react";
import Counter from "~/components/Counter/Counter";

export default function FileCount() {
    const [filecount, setFilecount] = useState<number>(0);

    useEffect(() => {
        const fetchData = async () => {
            fetch("http://localhost:5175/api/admin/files/count", {
                method: "GET",
                credentials: "include",
            }).then(response => response.json()).then(data => setFilecount(data.fileCount));
        };

        fetchData();

        const interval = setInterval(() => {
            fetchData();
        }, 5000);

        return () => clearInterval(interval);
    }, []);

    if (!filecount) {
        return <Counter title="Total Files" count={0} />;
    }
    
    return <Counter title="Total Files" count={filecount} />;
}