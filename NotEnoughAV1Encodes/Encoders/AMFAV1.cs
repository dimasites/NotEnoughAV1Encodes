﻿namespace NotEnoughAV1Encodes.Encoders
{
    internal class AMFAV1
    {
        /*
         * The implementation is pretty questionable
         * There is basically no documentation available online
         * Some unassuring things I found:
         *      - https://trac.ffmpeg.org/ticket/10389 (av1_amf ignores most quality/bitrate settings except for b:v)
         *      - https://trac.ffmpeg.org/ticket/10266 (Issues when using av1_amf to encode a video)*
         *        *does this mean that amd amf only supports 1080p!?
         *      - Supported pixel formats: nv12 yuv420p d3d11 dxva2_vld (no 422/444 ... really?!)
         *  
         *  It's unclear which arguments should be mixed with which.
         */
        public static string GetCommand(int quality, int rc, string bitrate, int qp)
        {
            string command = "-c:v av1_amf";

            // Speed
            //  -quality<int>                    E..V....... Set the encoding quality(from 0 to 100) (default speed)
            //      balanced        70           E..V.......
            //      speed           100          E..V.......
            //      quality         30           E..V.......
            //      high_quality    0            E..V.......
            command += " -quality ";
            command += quality switch
            {
                0 => "high_quality",
                1 => "quality",
                2 => "balanced",
                3 => "speed",
                _ => ""
            };


            // Quality Preset
            //  -rc<int>                         E..V....... Set the rate control mode(from - 1 to 6)(default - 1)
            //      cqp             0            E..V....... Constant Quantization Parameter
            //      vbr_latency     1            E..V....... Latency Constrained Variable Bitrate
            //      vbr_peak        2            E..V....... Peak Contrained Variable Bitrate
            //      cbr             3            E..V....... Constant Bitrate
            //      qvbr            4            E..V....... Quality Variable Bitrate
            //      hqvbr           5            E..V....... High Quality Variable Bitrate
            //      hqcbr           6            E..V....... High Quality Constant Bitrate
            command += " -rc ";
            command += rc switch
            {
                0 => "cqp -qp " + qp,
                1 => "cbr -b:v " + bitrate + "k",
                2 => "hqcbr -b:v " + bitrate + "k",
                3 => "qvbr -b:v " + bitrate + "k  -qvbr_quality_level " + qp,
                4 => "hqvbr -b:v " + bitrate + "k",
                _ => ""
            };

            return command;
        }

        public static string GetSpeed(double value)
        {
            return value switch
            {
                0 => "High Quality",
                1 => "Quality",
                2 => "Balanced",
                3 => "Speed",
                _ => "default"
            };
        }
    }
}
