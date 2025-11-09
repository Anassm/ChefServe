import FileDisplayer from "~/components/FileItem/FileDisplayer";
import type { fileItem } from "~/components/FileItem/FileItem";

export default function UserDashboard({ files }: { files: fileItem[] }) {
  return <FileDisplayer files={files} />;
}
