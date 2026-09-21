import React from "react";

export const StatCard = ({
  title,
  value,
  icon: Icon,
  color = "indigo",
  subtitle,
}) => {
  const colorMap = {
    indigo: "bg-indigo-500/10 text-indigo-400 border-indigo-500/20",
    emerald: "bg-emerald-500/10 text-emerald-400 border-emerald-500/20",
    amber: "bg-amber-500/10 text-amber-400 border-amber-500/20",
    rose: "bg-rose-500/10 text-rose-400 border-rose-500/20",
    cyan: "bg-cyan-500/10 text-cyan-400 border-cyan-500/20",
    purple: "bg-purple-500/10 text-purple-400 border-purple-500/20",
  };

  const selectedColor = colorMap[color] || colorMap.indigo;

  return (
    <div className="min-w-0 overflow-hidden bg-slate-800/80 backdrop-blur-xl border border-slate-700/60 rounded-2xl p-5 shadow-xl transition-all duration-300 hover:border-slate-600 hover:-translate-y-0.5">
      {/* Title */}
      <p className="whitespace-nowrap text-xs font-semibold uppercase tracking-wider text-slate-400">
        {title}
      </p>

      {/* Value + Icon */}
      <div className="mt-2 flex items-center justify-between gap-3">
        <h3 className="text-3xl font-extrabold text-white">{value}</h3>

        {Icon && (
          <div
            className={`flex h-12 w-12 shrink-0 items-center justify-center rounded-xl border ${selectedColor}`}
          >
            <Icon className="h-6 w-6" />
          </div>
        )}
      </div>

      {/* Subtitle */}
      {subtitle && <p className="mt-1 text-xs text-slate-400">{subtitle}</p>}
    </div>
  );
};
