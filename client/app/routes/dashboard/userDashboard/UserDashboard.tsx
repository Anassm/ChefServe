import type { Route } from "../../../+types/root";
import styles from "./UserDashboard.module.css";
import type { fileItem } from "~/components/FileItem/FileItem";
import { FileItem } from "~/components/FileItem/FileItem";
import FileDisplayer from "~/components/FileItem/FileDisplayer";
import type { LoaderFunctionArgs } from "react-router";
import { useState } from "react";
import type { TreeItem } from "~/components/FileTree/FileTree";




export default function UserDashboard({ files }: { files: fileItem[]}) {
  return <FileDisplayer files={files} />;
}