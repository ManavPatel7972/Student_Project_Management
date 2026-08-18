import React, { useState, useEffect } from 'react';
import { getTasksApi } from '../../api/tasks';
import { LoadingSpinner } from '../../components/common/LoadingSpinner';
import { Award, GraduationCap, FolderGit2 } from 'lucide-react';

export const ScoresPage = () => {
  const [tasks, setTasks] = useState([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    fetchTasks();
  }, []);

  const fetchTasks = async () => {
    try {
      setLoading(true);
      const data = await getTasksApi();
      setTasks(data);
    } catch (err) {
      console.error('Failed to load scores:', err);
    } finally {
      setLoading(false);
    }
  };

  // Group by student
  const studentMap = {};
  tasks.forEach((t) => {
    const sId = t.studentId || 0;
    if (!studentMap[sId]) {
      studentMap[sId] = {
        studentId: sId,
        studentName: t.studentName || 'Student',
        projectTitle: t.projectTitle || 'Project',
        totalAssigned: 0,
        totalEarned: 0,
        tasks: [],
      };
    }
    studentMap[sId].totalAssigned += t.assignedScore || 0;
    studentMap[sId].totalEarned += t.earnedScore || 0;
    studentMap[sId].tasks.push(t);
  });

  const studentScores = Object.values(studentMap);

  return (
    <div className="space-y-6 animate-fadeIn">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-2xl font-extrabold text-white">Student Score Reports</h1>
          <p className="text-xs text-slate-400">Cumulative task scores, earned marks & performance evaluation</p>
        </div>
        <div className="p-2.5 bg-amber-500/10 border border-amber-500/20 text-amber-400 rounded-xl text-xs font-bold flex items-center gap-2">
          <Award className="w-4 h-4" /> Score Evaluation
        </div>
      </div>

      {loading ? (
        <LoadingSpinner text="Calculating scores..." />
      ) : (
        <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
          {studentScores.map((st) => {
            const percentage =
              st.totalAssigned === 0 ? 0 : Math.round((st.totalEarned * 100) / st.totalAssigned);

            return (
              <div
                key={st.studentId}
                className="bg-slate-800/60 border border-slate-700/60 rounded-2xl p-6 backdrop-blur-xl shadow-xl space-y-4"
              >
                <div className="flex items-center justify-between">
                  <div className="flex items-center gap-3">
                    <div className="w-10 h-10 rounded-xl bg-gradient-to-br from-amber-500 to-orange-600 flex items-center justify-center font-bold text-white shadow-inner">
                      <GraduationCap className="w-5 h-5" />
                    </div>
                    <div>
                      <h3 className="font-bold text-white text-base">{st.studentName}</h3>
                      <p className="text-xs text-slate-400 flex items-center gap-1">
                        <FolderGit2 className="w-3 h-3 text-slate-500" /> {st.projectTitle}
                      </p>
                    </div>
                  </div>
                  <div className="text-right">
                    <p className="text-2xl font-extrabold text-amber-400">
                      {st.totalEarned} <span className="text-xs text-slate-500 font-medium">/ {st.totalAssigned}</span>
                    </p>
                    <p className="text-[10px] font-bold text-slate-400 uppercase">{percentage}% Score</p>
                  </div>
                </div>

                {/* Progress bar */}
                <div className="w-full bg-slate-950 h-2.5 rounded-full overflow-hidden border border-slate-800">
                  <div
                    className="bg-gradient-to-r from-amber-500 to-emerald-500 h-full rounded-full transition-all duration-500"
                    style={{ width: `${percentage}%` }}
                  ></div>
                </div>

                {/* Task Breakdown list */}
                <div className="pt-3 border-t border-slate-700/60 space-y-2">
                  <p className="text-[10px] font-bold text-slate-400 uppercase tracking-wider">
                    Task Score Items ({st.tasks.length})
                  </p>
                  {st.tasks.map((task) => (
                    <div
                      key={task.id}
                      className="flex items-center justify-between p-2.5 rounded-xl bg-slate-950/60 border border-slate-800/80 text-xs"
                    >
                      <div>
                        <p className="font-semibold text-white">{task.taskTitle}</p>
                        {task.facultyRemarks && (
                          <p className="text-[11px] text-indigo-400 italic">"{task.facultyRemarks}"</p>
                        )}
                      </div>
                      <span className="font-bold text-white">
                        <span className="text-emerald-400">{task.earnedScore ?? 0}</span> / {task.assignedScore}
                      </span>
                    </div>
                  ))}
                </div>
              </div>
            );
          })}
          {studentScores.length === 0 && (
            <div className="col-span-full py-12 text-center text-slate-500">
              No score records found.
            </div>
          )}
        </div>
      )}
    </div>
  );
};
