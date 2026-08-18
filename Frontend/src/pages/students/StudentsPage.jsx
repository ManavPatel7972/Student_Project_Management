import React, { useState, useEffect } from 'react';
import { getUsersApi } from '../../api/users';
import { Badge } from '../../components/common/Badge';
import { LoadingSpinner } from '../../components/common/LoadingSpinner';
import { GraduationCap, Mail, Phone, Building } from 'lucide-react';

export const StudentsPage = () => {
  const [students, setStudents] = useState([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    fetchStudents();
  }, []);

  const fetchStudents = async () => {
    try {
      setLoading(true);
      const data = await getUsersApi({ userTypeId: 3 }); // 3 = Student
      setStudents(data);
    } catch (err) {
      console.error('Failed to load students:', err);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="space-y-6 animate-fadeIn">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-2xl font-extrabold text-white">Enrolled Students</h1>
          <p className="text-xs text-slate-400">Overview of all active students and enrollment details</p>
        </div>
        <div className="p-2.5 bg-emerald-500/10 border border-emerald-500/20 text-emerald-400 rounded-xl text-xs font-bold flex items-center gap-2">
          <GraduationCap className="w-4 h-4" /> {students.length} Enrolled
        </div>
      </div>

      {loading ? (
        <LoadingSpinner text="Fetching students directory..." />
      ) : (
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
          {students.map((s) => (
            <div
              key={s.id}
              className="bg-slate-800/60 border border-slate-700/60 rounded-2xl p-5 backdrop-blur-xl flex flex-col justify-between hover:border-slate-600 transition-all"
            >
              <div>
                <div className="flex items-center justify-between mb-3">
                  <Badge variant="success">{s.userCode || 'STUDENT'}</Badge>
                  <Badge variant={s.isActive ? 'info' : 'danger'}>
                    {s.isActive ? 'Active' : 'Inactive'}
                  </Badge>
                </div>
                <div className="flex items-center gap-3 mb-4">
                  {s.profilePicturePath ? (
                    <img
                      src={s.profilePicturePath}
                      alt={s.fullName}
                      className="w-11 h-11 rounded-xl object-cover border border-slate-700"
                    />
                  ) : (
                    <div className="w-11 h-11 rounded-xl bg-gradient-to-br from-emerald-500 to-teal-600 flex items-center justify-center font-bold text-white shadow-inner">
                      {s.fullName.charAt(0)}
                    </div>
                  )}
                  <div>
                    <h3 className="font-bold text-white text-base">{s.fullName}</h3>
                    <p className="text-xs text-slate-400 flex items-center gap-1">
                      <Building className="w-3 h-3 text-slate-500" /> {s.departmentName || 'General Dept'}
                    </p>
                  </div>
                </div>
                <div className="space-y-1.5 pt-3 border-t border-slate-700/60 text-xs text-slate-300">
                  <p className="flex items-center gap-2">
                    <Mail className="w-3.5 h-3.5 text-slate-500" /> {s.email}
                  </p>
                  <p className="flex items-center gap-2">
                    <Phone className="w-3.5 h-3.5 text-slate-500" /> {s.mobileNumber || 'N/A'}
                  </p>
                </div>
              </div>
            </div>
          ))}
          {students.length === 0 && (
            <div className="col-span-full py-12 text-center text-slate-500">
              No enrolled students found.
            </div>
          )}
        </div>
      )}
    </div>
  );
};
