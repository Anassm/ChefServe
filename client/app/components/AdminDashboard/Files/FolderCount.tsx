import { useEffect, useState } from "react";
import Counter from "~/components/Counter/Counter";

export default function FolderCount() {
    const [foldercount, setFoldercount] = useState<number>(0);

    useEffect(() => {
        const fetchData = async () => {
            fetch("http://localhost:5175/api/admin/folders/count", {
                method: "GET",
                credentials: "include",
            }).then(response => response.json()).then(data => {setFoldercount(data.folderCount);});
        };

        fetchData();

        const interval = setInterval(() => {
            fetchData();
        }, 5000);

        return () => clearInterval(interval);

    }, []);

    if (!foldercount) {
        return <Counter title="Total Folders" count={0} />;
    }
    
    return <Counter title="Total Folders" count={foldercount} />;
}