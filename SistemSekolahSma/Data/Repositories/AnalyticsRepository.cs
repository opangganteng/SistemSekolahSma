using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System.Linq;
using SistemSekolahSMA.Data;
using System.Diagnostics;

namespace SistemSekolahSMA.Repositories
{
    public class AnalyticsRepository : IAnalyticsRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public AnalyticsRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        // ===== EXISTING INTERFACE METHODS (MUST IMPLEMENT) =====

        public async Task<dynamic> GetDashboardAnalyticsAsync()
        {
            Debug.WriteLine("=== AnalyticsRepository.GetDashboardAnalyticsAsync ===");

            var result = new
            {
                TotalSiswa = await GetTotalSiswaAsync(),
                TotalGuru = await GetTotalGuruAsync(),
                TotalKelas = await GetTotalKelasAsync(),
                KehadiranHariIni = await GetKehadiranHariIniAsync()
            };

            Debug.WriteLine($"✅ Repository Results: Siswa={result.TotalSiswa}, Guru={result.TotalGuru}, Kelas={result.TotalKelas}");
            return result;
        }

        public async Task<List<dynamic>> GetTrendKehadiranMingguanAsync()
        {
            return await GetTrendKehadiranMingguan();
        }

        public async Task<List<dynamic>> GetDistribusiSiswaAsync()
        {
            return await GetDistribusiSiswaPerTingkat();
        }

        public async Task<List<dynamic>> GetKehadiranMataPelajaranAsync()
        {
            return await GetKehadiranPerMataPelajaran();
        }

        public async Task<List<dynamic>> GetPerbandinganBulananAsync()
        {
            return await GetPerbandinganBulanan();
        }

        public async Task<List<dynamic>> GetKelasTopPerformerAsync()
        {
            return await GetRankingMataPelajaran();
        }

        public async Task<List<dynamic>> SearchJadwalByKelasAsync(string kelas)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = @"
                SELECT 
                    j.JadwalId,
                    k.NamaKelas,
                    mp.NamaMapel,
                    j.Hari,
                    j.JamMulai,
                    j.JamSelesai,
                    j.Ruangan
                FROM Jadwal j
                INNER JOIN Kelas k ON j.KelasId = k.KelasId
                INNER JOIN MataPelajaran mp ON j.MataPelajaranId = mp.MataPelajaranId
                WHERE k.NamaKelas LIKE @Kelas AND j.IsActive = 1
                ORDER BY j.Hari, j.JamMulai";

            return (await connection.QueryAsync<dynamic>(sql, new { Kelas = $"%{kelas}%" })).ToList();
        }

        // ===== REAL DATA METHODS =====

        public async Task<int> GetTotalSiswaAsync()
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                var sql = "SELECT COUNT(*) FROM Siswa WHERE IsActive = 1";
                var result = await connection.QueryFirstOrDefaultAsync<int>(sql);
                Debug.WriteLine($"✅ GetTotalSiswaAsync: {result}");
                return result;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ ERROR GetTotalSiswaAsync: {ex.Message}");
                return 0;
            }
        }

        public async Task<int> GetTotalGuruAsync()
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                var sql = "SELECT COUNT(*) FROM Guru WHERE IsActive = 1";
                var result = await connection.QueryFirstOrDefaultAsync<int>(sql);
                Debug.WriteLine($"✅ GetTotalGuruAsync: {result}");
                return result;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ ERROR GetTotalGuruAsync: {ex.Message}");
                return 0;
            }
        }

        public async Task<int> GetTotalKelasAsync()
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                var sql = "SELECT COUNT(*) FROM Kelas WHERE IsActive = 1";
                var result = await connection.QueryFirstOrDefaultAsync<int>(sql);
                Debug.WriteLine($"✅ GetTotalKelasAsync: {result}");
                return result;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ ERROR GetTotalKelasAsync: {ex.Message}");
                return 0;
            }
        }

        public async Task<decimal> GetKehadiranHariIniAsync()
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                var sql = @"
                    DECLARE @TotalPresensiHariIni INT = (
                        SELECT COUNT(*) 
                        FROM PresensiSiswa ps 
                        WHERE CAST(ps.TanggalPresensi AS DATE) = CAST(GETDATE() AS DATE)
                    )
                    DECLARE @HadirHariIni INT = (
                        SELECT COUNT(*) 
                        FROM PresensiSiswa ps 
                        WHERE CAST(ps.TanggalPresensi AS DATE) = CAST(GETDATE() AS DATE)
                        AND ps.StatusKehadiran = 'Hadir'
                    )
                    
                    SELECT CASE 
                        WHEN @TotalPresensiHariIni = 0 THEN 0 
                        ELSE CAST((@HadirHariIni * 100.0) / @TotalPresensiHariIni AS DECIMAL(5,2))
                    END";

                var result = await connection.QueryFirstOrDefaultAsync<decimal>(sql);
                Debug.WriteLine($"✅ GetKehadiranHariIniAsync: {result}%");
                return result;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ ERROR GetKehadiranHariIniAsync: {ex.Message}");
                return 0;
            }
        }

        public async Task<List<dynamic>> GetTrendKehadiranMingguan()
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();

                // Simple approach - get last 7 days with basic data
                var sql = @"
                    WITH Last7Days AS (
                        SELECT 
                            DATEADD(DAY, -6, GETDATE()) as StartDate,
                            GETDATE() as EndDate
                    ),
                    DayRange AS (
                        SELECT 0 as DayOffset
                        UNION ALL SELECT 1 UNION ALL SELECT 2 UNION ALL SELECT 3 
                        UNION ALL SELECT 4 UNION ALL SELECT 5 UNION ALL SELECT 6
                    ),
                    Days AS (
                        SELECT 
                            CAST(DATEADD(DAY, dr.DayOffset, l7.StartDate) AS DATE) as TanggalHari,
                            CASE DATENAME(WEEKDAY, DATEADD(DAY, dr.DayOffset, l7.StartDate))
                                WHEN 'Monday' THEN 'Senin'
                                WHEN 'Tuesday' THEN 'Selasa'
                                WHEN 'Wednesday' THEN 'Rabu'
                                WHEN 'Thursday' THEN 'Kamis'
                                WHEN 'Friday' THEN 'Jumat'
                                WHEN 'Saturday' THEN 'Sabtu'
                                WHEN 'Sunday' THEN 'Minggu'
                            END as Day
                        FROM Last7Days l7
                        CROSS JOIN DayRange dr
                    )
                    SELECT 
                        d.Day,
                        COALESCE(
                            CASE 
                                WHEN COUNT(ps.PresensiSiswaId) = 0 THEN 0
                                ELSE CAST((COUNT(CASE WHEN ps.StatusKehadiran = 'Hadir' THEN 1 END) * 100.0) / COUNT(ps.PresensiSiswaId) AS DECIMAL(5,2))
                            END, 0
                        ) as SiswaAttendance,
                        COALESCE(
                            CASE 
                                WHEN COUNT(pg.PresensiGuruId) = 0 THEN 0
                                ELSE CAST((COUNT(CASE WHEN pg.StatusKehadiran = 'Hadir' THEN 1 END) * 100.0) / COUNT(pg.PresensiGuruId) AS DECIMAL(5,2))
                            END, 0
                        ) as GuruAttendance
                    FROM Days d
                    LEFT JOIN PresensiSiswa ps ON CAST(ps.TanggalPresensi AS DATE) = d.TanggalHari
                    LEFT JOIN PresensiGuru pg ON CAST(pg.TanggalPresensi AS DATE) = d.TanggalHari
                    GROUP BY d.TanggalHari, d.Day
                    ORDER BY d.TanggalHari";

                var result = await connection.QueryAsync<dynamic>(sql);
                Debug.WriteLine($"✅ GetTrendKehadiranMingguan: {result.Count()} days");
                return result.ToList();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ ERROR GetTrendKehadiranMingguan: {ex.Message}");
                // Return default 7 days with 0 values
                return new List<dynamic>
                {
                    new { Day = "Senin", SiswaAttendance = 0, GuruAttendance = 0 },
                    new { Day = "Selasa", SiswaAttendance = 0, GuruAttendance = 0 },
                    new { Day = "Rabu", SiswaAttendance = 0, GuruAttendance = 0 },
                    new { Day = "Kamis", SiswaAttendance = 0, GuruAttendance = 0 },
                    new { Day = "Jumat", SiswaAttendance = 0, GuruAttendance = 0 },
                    new { Day = "Sabtu", SiswaAttendance = 0, GuruAttendance = 0 },
                    new { Day = "Minggu", SiswaAttendance = 0, GuruAttendance = 0 }
                };
            }
        }

        public async Task<List<dynamic>> GetDistribusiSiswaPerTingkat()
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                var sql = @"
                    SELECT 
                        CASE k.Tingkat 
                            WHEN 10 THEN 'Kelas X'
                            WHEN 11 THEN 'Kelas XI' 
                            WHEN 12 THEN 'Kelas XII'
                            ELSE 'Kelas ' + CAST(k.Tingkat AS VARCHAR)
                        END as Label,
                        COUNT(s.SiswaId) as Count,
                        k.Tingkat
                    FROM Kelas k
                    LEFT JOIN Siswa s ON k.KelasId = s.KelasId AND s.IsActive = 1
                    WHERE k.IsActive = 1
                    GROUP BY k.Tingkat
                    HAVING COUNT(s.SiswaId) > 0
                    ORDER BY k.Tingkat";

                var result = await connection.QueryAsync<dynamic>(sql);
                Debug.WriteLine($"✅ GetDistribusiSiswaPerTingkat: {result.Count()} levels");
                return result.ToList();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ ERROR GetDistribusiSiswaPerTingkat: {ex.Message}");
                return new List<dynamic>();
            }
        }

        public async Task<List<dynamic>> GetKehadiranPerMataPelajaran()
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                var sql = @"
                    SELECT TOP 6
                        mp.NamaMapel as Subject,
                        COALESCE(COUNT(ps.PresensiSiswaId), 0) as TotalPresensi,
                        COALESCE(COUNT(CASE WHEN ps.StatusKehadiran = 'Hadir' THEN 1 END), 0) as TotalHadir,
                        CASE 
                            WHEN COUNT(ps.PresensiSiswaId) = 0 THEN 0 
                            ELSE CAST((COUNT(CASE WHEN ps.StatusKehadiran = 'Hadir' THEN 1 END) * 100.0) / COUNT(ps.PresensiSiswaId) AS DECIMAL(5,2))
                        END as Percentage
                    FROM MataPelajaran mp
                    LEFT JOIN Jadwal j ON mp.MataPelajaranId = j.MataPelajaranId AND j.IsActive = 1
                    LEFT JOIN PresensiSiswa ps ON j.JadwalId = ps.JadwalId
                    WHERE mp.IsActive = 1
                    GROUP BY mp.MataPelajaranId, mp.NamaMapel
                    ORDER BY Percentage DESC, mp.NamaMapel";

                var result = await connection.QueryAsync<dynamic>(sql);
                Debug.WriteLine($"✅ GetKehadiranPerMataPelajaran: {result.Count()} subjects");
                return result.ToList();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ ERROR GetKehadiranPerMataPelajaran: {ex.Message}");
                return new List<dynamic>();
            }
        }

        public async Task<List<dynamic>> GetPerbandinganBulanan()
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();

                // Simple approach - get current year monthly data
                var sql = @"
                    WITH Months AS (
                        SELECT 1 as BulanNum, 'Jan' as Month
                        UNION ALL SELECT 2, 'Feb' UNION ALL SELECT 3, 'Mar'
                        UNION ALL SELECT 4, 'Apr' UNION ALL SELECT 5, 'Mei'
                        UNION ALL SELECT 6, 'Jun' UNION ALL SELECT 7, 'Jul'
                        UNION ALL SELECT 8, 'Ags' UNION ALL SELECT 9, 'Sep'
                        UNION ALL SELECT 10, 'Okt' UNION ALL SELECT 11, 'Nov'
                        UNION ALL SELECT 12, 'Des'
                    ),
                    MonthlyData AS (
                        SELECT 
                            m.Month,
                            m.BulanNum,
                            COALESCE(
                                CASE 
                                    WHEN COUNT(ps.PresensiSiswaId) = 0 THEN 0
                                    ELSE CAST((COUNT(CASE WHEN ps.StatusKehadiran = 'Hadir' THEN 1 END) * 100.0) / COUNT(ps.PresensiSiswaId) AS DECIMAL(5,2))
                                END, 0
                            ) as SiswaAttendance,
                            COALESCE(
                                CASE 
                                    WHEN COUNT(pg.PresensiGuruId) = 0 THEN 0
                                    ELSE CAST((COUNT(CASE WHEN pg.StatusKehadiran = 'Hadir' THEN 1 END) * 100.0) / COUNT(pg.PresensiGuruId) AS DECIMAL(5,2))
                                END, 0
                            ) as GuruAttendance
                        FROM Months m
                        LEFT JOIN PresensiSiswa ps ON MONTH(ps.TanggalPresensi) = m.BulanNum AND YEAR(ps.TanggalPresensi) = YEAR(GETDATE())
                        LEFT JOIN PresensiGuru pg ON MONTH(pg.TanggalPresensi) = m.BulanNum AND YEAR(pg.TanggalPresensi) = YEAR(GETDATE())
                        WHERE m.BulanNum <= MONTH(GETDATE())
                        GROUP BY m.Month, m.BulanNum
                    )
                    SELECT * FROM MonthlyData
                    ORDER BY BulanNum";

                var result = await connection.QueryAsync<dynamic>(sql);
                Debug.WriteLine($"✅ GetPerbandinganBulanan: {result.Count()} months");
                return result.ToList();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ ERROR GetPerbandinganBulanan: {ex.Message}");
                return new List<dynamic>();
            }
        }

        public async Task<List<dynamic>> GetRankingMataPelajaran()
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                var sql = @"
                    SELECT TOP 5
                        mp.NamaMapel,
                        COALESCE(COUNT(ps.PresensiSiswaId), 0) as TotalPresensi,
                        CASE 
                            WHEN COUNT(ps.PresensiSiswaId) = 0 THEN 0 
                            ELSE CAST((COUNT(CASE WHEN ps.StatusKehadiran = 'Hadir' THEN 1 END) * 100.0) / COUNT(ps.PresensiSiswaId) AS DECIMAL(5,2))
                        END as AttendancePercentage
                    FROM MataPelajaran mp
                    LEFT JOIN Jadwal j ON mp.MataPelajaranId = j.MataPelajaranId AND j.IsActive = 1
                    LEFT JOIN PresensiSiswa ps ON j.JadwalId = ps.JadwalId
                    WHERE mp.IsActive = 1
                    GROUP BY mp.MataPelajaranId, mp.NamaMapel
                    ORDER BY AttendancePercentage DESC, mp.NamaMapel";

                var result = await connection.QueryAsync<dynamic>(sql);
                Debug.WriteLine($"✅ GetRankingMataPelajaran: {result.Count()} subjects");
                return result.ToList();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ ERROR GetRankingMataPelajaran: {ex.Message}");
                return new List<dynamic>();
            }
        }

        public async Task<List<dynamic>> GetStatistikKehadiranGuru()
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                var sql = @"
                    SELECT 
                        COALESCE(COUNT(pg.PresensiGuruId), 0) as TotalPresensiGuru,
                        COALESCE(COUNT(CASE WHEN pg.StatusKehadiran = 'Hadir' THEN 1 END), 0) as TotalHadirGuru,
                        COALESCE(COUNT(CASE WHEN pg.StatusKehadiran = 'Tidak Hadir' THEN 1 END), 0) as TotalTidakHadirGuru,
                        CASE 
                            WHEN COUNT(pg.PresensiGuruId) = 0 THEN 0 
                            ELSE CAST((COUNT(CASE WHEN pg.StatusKehadiran = 'Hadir' THEN 1 END) * 100.0) / COUNT(pg.PresensiGuruId) AS DECIMAL(5,2))
                        END as PersentaseKehadiranGuru
                    FROM PresensiGuru pg
                    WHERE pg.TanggalPresensi >= DATEADD(DAY, -30, GETDATE())";

                var result = await connection.QueryAsync<dynamic>(sql);
                Debug.WriteLine($"✅ GetStatistikKehadiranGuru: {result.Count()} records");
                return result.ToList();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ ERROR GetStatistikKehadiranGuru: {ex.Message}");
                return new List<dynamic>();
            }
        }

        public async Task<List<dynamic>> GetTrendBulananSiswa()
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                var sql = @"
                    SELECT 
                        MONTH(s.CreatedDate) as Bulan,
                        YEAR(s.CreatedDate) as Tahun,
                        COUNT(s.SiswaId) as JumlahSiswa
                    FROM Siswa s
                    WHERE s.CreatedDate >= DATEADD(MONTH, -12, GETDATE()) AND s.IsActive = 1
                    GROUP BY MONTH(s.CreatedDate), YEAR(s.CreatedDate)
                    ORDER BY Tahun, Bulan";

                var result = await connection.QueryAsync<dynamic>(sql);
                Debug.WriteLine($"✅ GetTrendBulananSiswa: {result.Count()} months");
                return result.ToList();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ ERROR GetTrendBulananSiswa: {ex.Message}");
                return new List<dynamic>();
            }
        }
    }
}