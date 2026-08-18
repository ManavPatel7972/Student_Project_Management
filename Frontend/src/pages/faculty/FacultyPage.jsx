import React, { useState, useEffect } from 'react';
import { getUsersApi } from '../../api/users';
import { Badge } from '../../components/common/Badge';
import { LoadingSpinner } from '../../components/common/LoadingSpinner';
import { Users, Mail, Phone, Building } from 'lucide-react';

export const FacultyPage = () => {
  const [faculty, setFaculty] = useState([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    fetchFaculty();
  }, []);

  const fetchFaculty = async () => {
    try {
      setLoading(true);
      const data = await getUsersApi({ userTypeId: 2 }); // 2 = Faculty
      setFaculty(data);
    } catch (err) {
      console.error('Failed to load faculty:', err);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="space-y-6 animate-fadeIn">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-2xl font-extrabold text-white">Faculty Members</h1>
          <p className="text-xs text-slate-400">Overview of project guides and academic mentors</p>
        </div>
        <div className="p-2.5 bg-purple-500/10 border border-purple-500/20 text-purple-400 rounded-xl text-xs font-bold flex items-center gap-2">
          <Users className="w-4 h-4" /> {faculty.length} Faculty Members
        </div>
      </div>

      {loading ? (
        <LoadingSpinner text="Fetching faculty directory..." />
      ) : (
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
          {faculty.map((f) => (
            <div
              key={f.id}
              className="bg-slate-800/60 border border-slate-700/60 rounded-2xl p-5 backdrop-blur-xl flex flex-col justify-between hover:border-slate-600 transition-all"
            >
              <div>
                <div className="flex items-center justify-between mb-3">
                  <Badge variant="purple">{f.userCode || 'FACULTY'}</Badge>
                  <Badge variant={f.isActive ? 'success' : 'danger'}>
                    {f.isActive ? 'Active' : 'Inactive'}
                  </Badge>
                </div>
                <div className="flex items-center gap-3 mb-4">
                  {f.profilePicturePath ? (
                    <img
                      src={f.profilePicturePath}
                      alt={f.fullName}
                      className="w-11 h-11 rounded-xl object-cover border border-slate-700"
                    />
                  ) : (
                    <div className="w-11 h-11 rounded-xl bg-gradient-to-br from-purple-500 to-indigo-600 flex items-center justify-center font-bold text-white shadow-inner">
                      {f.fullName.charAt(0)}
                    </div>
                  )}
                  <div>
                    <h3 className="font-bold text-white text-base">{f.fullName}</h3>
                    <p className="text-xs text-slate-400 flex items-center gap-1">
                      <Building className="w-3 h-3 text-slate-500" /> {f.departmentName || 'Computer Science'}
                    </p>
                  </div>
                </div>
                <div className="space-y-1.5 pt-3 border-t border-slate-700/60 text-xs text-slate-300">
                  <p className="flex items-center gap-2">
                    <Mail className="w-3.5 h-3.5 text-slate-500" /> {f.email}
                  </p>
                  <p className="flex items-center gap-2">
                    <Phone className="w-3.5 h-3.5 text-slate-500" /> {f.mobileNumber || 'N/A'}
                  </p>
                </div>
              </div>
            </div>
          ))}
          {faculty.length === 0 && (
            <div className="col-span-full py-12 text-center text-slate-500">
              No faculty members found.
            </div>
          )}
        </div>
      )}
    </div>
  );
};
