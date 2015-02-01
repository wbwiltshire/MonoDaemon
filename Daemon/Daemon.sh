#!/bin/sh
#
USR1=10
DESC="Simple Mono/C# Daemon"
NAME=Daemon.exe
DAEMON=$NAME
PIDFILE=Daemon.pid
start_daemon() {
    if (test -f $DAEMON); then
       if (test -f $PIDFILE); then 
          echo "$DESC" is already running!
       else
          mono "$DAEMON" &
          #pid=`pidof "$NAME"`
          pid=`pgrep -f "mono $NAME" `
          echo "$pid" > $PIDFILE
       fi
    else
       echo "$DAEMON" executable does not exist
       exit 1
    fi
}
status_daemon() {
    if (test -f $DAEMON); then
       if (test -f $PIDFILE); then 
          read pid < "$PIDFILE"
          echo "$DESC is running with pid: " "$pid"
       else
          echo "$DESC" is not running.
       fi
    else
       echo "$DAEMON" executable does not exist
       exit 1
    fi
}
term_daemon() {
    if (test -f $DAEMON); then
       if (test -f $PIDFILE); then 
          read pid < "$PIDFILE"
          kill -s $USR1 $pid
          rm -f $PIDFILE
       else
          echo "$DESC" is not running.
       fi
   else
       echo "$DAEMON" executable does not exist
       exit 1
   fi
}
log_daemon() {
    grep $LOGPREFIX /var/log/syslog
}
#main processing
case "$1" in 
    start)
       start_daemon $PIDFILE $DAEMON
       ;;
    status)
       status_daemon $PIDFILE $DAEMON
       ;;
    stop)
       term_daemon $PIDFILE $DAEMON
       ;;
    log)
       log_daemon
       ;;
    *)
       echo "Usage: <> {start|stop|status}"
       exit 1
       ;;
esac

exit 0
