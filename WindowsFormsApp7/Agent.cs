using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace WindowsFormsApp7
{
    public class Agent
    {
        public virtual double getNextEventTime()
        {
            return double.MaxValue;

        }
        public virtual void ProccessEvent()
        {
            
        }
    }

    public class ClientCustom: Agent //входящий поток
    {
        private Random rnd = new Random();
        public double lambda = 2;
        private double nexttimeclient = 0;
        private Restoraunt restoraunt;
        public ClientCustom(Restoraunt restoraunt)

        {
            this.restoraunt = restoraunt;
            nexttimeclient = simulateClientCustomTime();

        }

        private double simulateClientCustomTime()
        {
            return -Math.Log(rnd.NextDouble()) / lambda;
        }
        public override double getNextEventTime()
        {
            return nexttimeclient;
        }
        public override void ProccessEvent()
        {
            base.ProccessEvent();
            Customer customer = new Customer();
            restoraunt.CustomerA(customer);
            nexttimeclient += simulateClientCustomTime();
        }
    }

    public class Customer: Agent //клиенты
    {
        //inf for client time etc ok
    }

    public class MyQuere : Agent {
        private List<Operator> operators = new List<Operator>();
        private Queue<Customer> queue = new Queue<Customer>();
        internal void acceptCustomer(Customer customer)
        {
            queue.Enqueue(customer);


        }
        public bool hasCustomers()
        {
            return (queue.Count > 0);
        }
        public Customer takeCustomer()
        {
            return queue.Dequeue();
        }
        public int getQueueSize()
        {
            return queue.Count();
        }
    }


    public class Restoraunt : Agent
    {
        private Service service = new Service();
        private MyQuere quere = new MyQuere();
     
        public void CustomerA(Customer customer)
        {
            if (service.hasFreeOp()) service.acceptCustomer(customer);
            else quere.acceptCustomer(customer);
        }

        public override double getNextEventTime()
        {
            return service.getNextEventTime();
        }
        public override void ProcessEvent()
        {
            service.ProcessEvent();
            if (quere.hasCustomers())
            {
                Customer cus = quere.takeCustomer();
                service.acceptCustomer(cus);
            }
        }
    }


    public class Service : Agent
    {
        private List<Operator> operators = new List<Operator>();
        private Operator activeOper = new Operator();
        public Service(int N)
        {
            for (int i = 0; i < N; i++)
            {
                operators.Add(new Operator());
            }
        }
        public void acceptCustomer(Customer customer)
        {
            Operator op = findFreeOp();
            if (op != null) op.acceptCustomer(customer);
        }
        internal bool hasFreeOp()
        {
            Operator op = findFreeOp();
            return op != null;
        }

        private Operator findFreeOp()
        {
            foreach (Operator op in operators)
                if (op.isFree()) return op;
            return null;
        }

        //getNextEventTime(){счет t}
        //proccessEvent {if aO != 0 return aO.proccessEvent}
        public override double getNextEventTime()
        {
            double tMin = double.MaxValue;
            activeOper = null;
            foreach (Operator op in operators)
            {
                double tA = op.getNextEventTime();
                if (tA < tMin)
                {
                    tMin = tA;
                    activeOper = op;
                }
            }
            return tMin;
        }
        public override void processEvent()
        {
            if (activeOper != null)
                activeOper.processEvent();
        }

    }
    public class Operator : Agent
    {
        private double endOfSeviceTime = double.MaxValue;
        private double endOfDerviceTime = double.MaxValue; //время окончания обслуживания
        private Customer customerInService = null;
        internal void acceptCustomer(Customer customer)
        {
            if (isFree())
            {
                customerInService = customer;
                endOfDerviceTime = Model.Time + simulateServiceTime();
            }
        }

        public bool isFree()
        {
            return (customerInService == null);
        }

        public double lambda = 2;
        private Random rnd = new Random();
        private double simulateServiceTime()
        {
            return -Math.Log(rnd.NextDouble()) / lambda;
        }

        public override double getNextEventTime()
        {
            return endOfSeviceTime;
        }
        public override void ProcessEvent()
        {
            customerInService = null;
            endOfSeviceTime = double.MaxValue;
        }

    }
}

