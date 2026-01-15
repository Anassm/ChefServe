import { useEffect, useState } from "react";
import Counter from "~/components/Counter/Counter";

export default function EmptyFolderCount() {
    const [count, setCount] = useState<number>(0);

    useEffect(() => {
        const fetchData = async () => {
            fetch("http://localhost:5175/api/admin/folders/empty-count", {
                method: "GET",
                credentials: "include",
            }).then(response => response.json()).then(data => {setCount(data.emptyFolderCount);});
        };
        fetchData();
        const interval = setInterval(() => {
            fetchData();
        }, 5000);

        return () => clearInterval(interval);
    }, []);

    if (!count) {
        return <Counter title="Empty Folders" count={0} />;
    }

    return <Counter title="Empty Folders" count={count} />;
}
