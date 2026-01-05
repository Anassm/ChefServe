import { useEffect, useState } from "react";
import Counter from "~/components/Counter/Counter";

export default function EmptyFolderCount() {
    const [count, setCount] = useState<number>(0);

    useEffect(() => {
        const fetchData = async () => {
            try {
                const response = await fetch("http://localhost:5175/api/admin/folders/empty/count", {
                    method: "GET",
                    credentials: "include",
                });
                if (!response.ok) {
                    return;
                }
                const data = await response.json();
                setCount(data.emptyFolderCount ?? 0);
            } catch (error) {
                console.error("Failed to fetch empty folder count", error);
            }
        };

        fetchData();
        const interval = setInterval(fetchData, 5000);
        return () => clearInterval(interval);
    }, []);

    return <Counter title="Empty Folders" count={count || 0} />;
}
