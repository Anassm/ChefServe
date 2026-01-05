import { useEffect, useState } from "react";
import Counter from "~/components/Counter/Counter";

export default function FoldersWithContentCount() {
    const [count, setCount] = useState<number>(0);

    useEffect(() => {
        const fetchData = async () => {
            try {
                const response = await fetch("http://localhost:5175/api/admin/folders/with-content/count", {
                    method: "GET",
                    credentials: "include",
                });
                if (!response.ok) {
                    return;
                }
                const data = await response.json();
                setCount(data.foldersWithContentCount ?? 0);
            } catch (error) {
                console.error("Failed to fetch folders with content count", error);
            }
        };

        fetchData();
        const interval = setInterval(fetchData, 5000);
        return () => clearInterval(interval);
    }, []);

    return <Counter title="Folders With Files" count={count || 0} />;
}
