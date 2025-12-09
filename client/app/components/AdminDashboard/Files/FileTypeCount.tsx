import { useEffect, useState } from "react";
import Counter from "~/components/Counter/Counter";

export default function FileTypeCount() {
    const [fileTypeCount, setFileTypeCount] = useState<number>(0);
    useEffect(() => {
        fetch("http://localhost:5175/api/admin/filetypes/count", {
            method: "GET",
            credentials: "include",
        }).then(response => response.json()).then(data => {setFileTypeCount(data.fileTypeCount);});
    }, []);
    if (!fileTypeCount) {
        return <Counter title="Total File Types" count={0} />;
    }
    return <Counter title="Total File Types" count={fileTypeCount} />;
}