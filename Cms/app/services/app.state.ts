import {Injectable} from '@angular/core'
import {Subject}    from 'rxjs/Subject';

@Injectable()
export class AppState {

  private data = new Subject<Object>();
  private dataStream$ = this.data.asObservable();

  private subscriptions = new Map<string, Array<Function>>();

  constructor() {
      this.dataStream$.subscribe((data) => {
          this.onEvent(data);
      });
  }

  notifyDataChanged(event, value) {

    let current = this.data[event];
    // ReSharper disable once CoercedEqualsUsing
    if (current != value) {
      this.data[event] = value;

      this.data.next({
        event: event,
        data: this.data[event]
      });
    }
  }

  subscribe(event:string, callback:Function) {
    var subscribers = this.subscriptions.get(event) || [];
    subscribers.push(callback);

    this.subscriptions.set(event, subscribers);
  }

  onEvent(data:any) {
    var subscribers = this.subscriptions.get(data['event']) || [];

    subscribers.forEach((callback) => {
        callback(data['data']);
    });
  }
}
