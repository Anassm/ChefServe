import type { Route } from "../../../+types/root";
import styles from "./UserDashboard.module.css";
import type { fileItem } from "~/components/FileItem/FileItem";
import { FileItem } from "~/components/FileItem/FileItem";
import FileDisplayer from "~/components/FileItem/FileDisplayer";
import type { LoaderFunctionArgs } from "react-router";



export default function UserDashboard({ files } : { files: fileItem[] }) {
  console.log("Fetched files2:", files);
  console.log(files);
  return (
    <FileDisplayer files={files} />
  )
}