import { cn } from "@/lib/utils";

/**
 * Skeleton - Component hiển thị trạng thái loading
 */
function Skeleton({
  className,
  ...props
}) {
  return (
    <div
      className={cn(
        "animate-pulse rounded-md bg-muted",
        className
      )}
      {...props}
    />
  );
}

export { Skeleton };
